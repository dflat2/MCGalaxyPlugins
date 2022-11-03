using BlockID = System.UInt16;
using MCGalaxy.Commands;
using MCGalaxy.DB;
using System.Collections.Generic;
using MCGalaxy.Maths;
using MCGalaxy;
using MCGalaxy.Blocks;
using MCGalaxy.Commands.CPE;
using System;

namespace EasyFencesPlugin {
	
	internal partial class FenceSetWizard
	{
	    private string[] instructionsCanJumpOver =
	    { "Can the player jump over the fences? [yes/no]" };
	
	    private string[] instructionsDoBury =
	    { "Will the fence elements be buried? [yes/no]" };
	
	    private string[] instructionsSourceID =
	    { "Which block-id will the fences be copied from?" };
	
	    private string[] instructionsCrossIntersect =
	    { "Do you need +-shaped intersections? [yes/no]" };
	
	    private string[] instructionsTIntersect =
	    { "Do you need T-shaped intersections? [yes/no]" };
	
	    private string[] instructionsDestID =
	    {
	        "Type the target id for the post block",
	        "Others fence elements will be added consecutively."
	    };
	}
	
	internal class FenceElement
	{
	    private int offset = 0;
	    private ElementDirection direction = ElementDirection.X;
	    private ElementPosition position = ElementPosition.Top;
	    private BlockID copiedTo;
	    private ElementType type;
	
	    public BlockID copiedFrom;
	
	    internal FenceElement(ElementType type, BlockID copiedFrom, BlockID copiedTo,
	                          ElementDirection direction, ElementPosition position, int offset)
	    {
	        this.type = type;
	        this.copiedFrom = copiedFrom;
	        this.copiedTo = copiedTo;
	        this.direction = direction;
	        this.position = position;
	        this.offset = offset;
	    }
	
	    internal FenceElement(ElementType type, BlockID copiedFrom, BlockID copiedTo,
	                      ElementDirection direction)
	    {
	        this.type = type;
	        this.copiedFrom = copiedFrom;
	        this.copiedTo = copiedTo;
	        this.direction = direction;
	    }
	
	    internal FenceElement(ElementType type, BlockID copiedFrom, BlockID copiedTo)
	    {
	        this.type = type;
	        this.copiedFrom = copiedFrom;
	        this.copiedTo = copiedTo;
	    }
	
	    private AABB Aabb
	    {
	        get
	        {
	            switch (type)
	            {
	                case ElementType.Barrier:
	                    return ApplyOffset(FenceElementsAABBs.Barrier(direction, position));
	                case ElementType.Post:
	                case ElementType.AntiJumpOverCorner:
	                    return FenceElementsAABBs.Post();
	                case ElementType.Corner:
	                    return ApplyOffset(FenceElementsAABBs.Corner(direction, position));
	                case ElementType.AntiJumpOver:
	                    return FenceElementsAABBs.AntiJumpOver(direction);
	                default:
	                    return new AABB(0, 0, 0, 16, 16, 16);
	            }
	        }
	    }
	
	    private AABB ApplyOffset(AABB aabb)
	    {
	        int offsetUnits = 16 * offset;
	        return aabb.Offset(0, -offsetUnits, 0);
	    }
	
	    public override string ToString()
	    {
	        string direction = this.direction.ToString();
	        string position = this.position.ToString();
	
	        switch (type)
	        {
	            case ElementType.Post:
	                return "Fence post";
	            case ElementType.AntiJumpOver:
	                return $"Fence {direction} anti-jump";
	            case ElementType.AntiJumpOverCorner:
	                return "Fence anti-jump corner";
	            case ElementType.Barrier:
	            case ElementType.Corner:
	                return $"Fence {direction} {position} Offset {offset}";
	            default:
	                return "";
	        }
	    }
	
	    internal List<string> RawCommands(Player player, int count, string prefix)
	    {
	        int targetID = copiedTo + count;
	        List<string> commands = new List<string>();
	        commands.Add($"{prefix}copy {copiedFrom} {targetID}");
	
	        string minimum = Aabb.Min.ToStringNoComma();
	        string maximum = Aabb.Max.ToStringNoComma();
	
	        commands.Add($"{prefix}edit {targetID} min {minimum}");
	        commands.Add($"{prefix}edit {targetID} max {maximum}");
	        commands.Add($"{prefix}edit {targetID} blockslight no");
	
	        if (type == ElementType.AntiJumpOver || type == ElementType.AntiJumpOverCorner)
	            commands.Add($"{prefix}edit {targetID} blockdraw 4");
	
	        if (offset != 0)
	        {
	            BlockDefinition def = CopiedFromBD(player);
	            int sideTexID = def.FrontTex;
	
	            // Known issue: things will go wrong when wrapping if current texture pack has 512 textures
	            if (sideTexID - offset >= 0)
	                commands.Add($"{prefix}edit {targetID} sidetex {(sideTexID - offset) % 256}");
	            else
	                commands.Add($"{prefix}edit {targetID} sidetex {(sideTexID - offset) % 256 + 256}");
	        }
	
	        commands.Add($"{prefix}edit {targetID} name {this.ToString()}");
	
	        return commands;
	    }
	
	    private BlockDefinition CopiedFromBD(Player p)
	    {
	        Level lvl = p.Level;
	        BlockID serverBlockID = Block.FromRaw(copiedFrom);
	
	        bool levelBlockExists = (lvl.CustomBlockDefs[Block.FromRaw(copiedFrom)] != null);
	        bool globalBlockExists = (BlockDefinition.GlobalDefs[Block.FromRaw(copiedFrom)] != null);
	
	        if (levelBlockExists)
	            return lvl.CustomBlockDefs[serverBlockID];
	        else if (globalBlockExists)
	            return BlockDefinition.GlobalDefs[serverBlockID];
	        else
	            return DefaultSet.MakeCustomBlock(serverBlockID);
	    }
	}
	
	
	
	public class CmdEasyFences : Command2
	{
	    public override string name => "EasyFences";
	    public override string type => "Building";
	    public override string shortcut => "ezf";
	    public override bool SuperUseable => false;
	
	    private string usage = "&T/easyfences";
	    
	    public override void Help(Player p)
	    {
	        p.Message(usage);
	        p.Message("&HRun the Minecraft-fences creation process.");
	    }
	
	    public override void Use(Player p, string message)
	    {
	        bool hasWizard = p.Extras.Contains("FenceSetWizard");
	        FenceSetWizard wizard;
	
	        if (!hasWizard)
	        {
	            wizard = new FenceSetWizard(p);
	            p.Extras["FenceSetWizard"] = wizard;
	            return;
	        }
	
	        wizard = (FenceSetWizard)p.Extras["FenceSetWizard"];
	        string wizardInput = message.SplitSpaces()[0];
	
	        if (wizardInput.ToLower() == "abort" || wizardInput.ToLower() == "cancel")
	        {
	            p.Extras.Remove("FenceSetWizard");
	            p.Message("&SAborted the fence creation process.");
	            return;
	        }
	
	        bool wizardEnd = wizard.ManageInput(wizardInput);
	
	        if (wizardEnd)
	        {
	            List<FenceElement> elements = wizard.BuildFenceElements();
	            AddFencesElements(p, elements);
	            p.Extras.Remove("FenceSetWizard");
	        }
	    }
	
	    private void AddFencesElements(Player p, List<FenceElement> elements)
	    {
	        Command cmdLevelBlock = Command.Find("levelblock");
	        Command cmdOverseer = Command.Find("overseer");
	
	        Command lbCmd = cmdLevelBlock;
	        string prefix = "";
	
	        if (!p.CanUse(cmdLevelBlock))
	        {
	            if (LevelInfo.IsRealmOwner(p.Level, p.name))
	            {
	                lbCmd = cmdOverseer;
	                prefix = "lb ";
	            }
	            else
	            {
	                p.Message("&WYou do not have the permissions to edit level blocks on this map.");
	                return;
	            }
	        }
	
	        List<string> rawCommands;
	
	        for (int i = 0; i < elements.Count; i++)
	        {
	            rawCommands = elements[i].RawCommands(player: p, count: i, prefix: prefix);
	
	            foreach (string command in rawCommands)
	            {
	                lbCmd.Use(p, command);
	            }
	        }
	    }
	}
	
	
	internal enum ElementType
	{
	    Post,
	    Corner,
	    Barrier,
	    AntiJumpOver,
	    AntiJumpOverCorner
	}
	
	internal enum ElementDirection
	{
	    X,
	    Z
	}
	
	internal enum ElementPosition
	{
	    // For barriers
	    Top,
	    Bottom,
	    // For corners
	    TopRight,
	    TopLeft,
	    BottomRight,
	    BottomLeft
	}	
	internal static class FenceElementsAABBs
	{
	    private const int BOTTOM_BARRIER_MIN_Y = 6;
	    private const int BOTTOM_BARRIER_MAX_Y = 9;
	    private const int TOP_BARRIER_MIN_Y = 12;
	    private const int TOP_BARRIER_MAX_Y = 15;
	
	    private static AABB DEFAULT_AABB = new AABB(0, 0, 0, 16, 16, 16);
	
	    internal static AABB Post()
	    {
	        return new AABB(6, 0, 6, 10, 16, 10);
	    }
	
	    internal static AABB Corner(ElementDirection direction, ElementPosition position)
	    {
	        switch (direction)
	        {
	            case (ElementDirection.X):
	                switch (position)
	                {
	                    case (ElementPosition.BottomLeft):
	                        return new AABB(0, BOTTOM_BARRIER_MIN_Y, 7, 6, BOTTOM_BARRIER_MAX_Y, 9);
	                    case (ElementPosition.BottomRight):
	                        return new AABB(10, BOTTOM_BARRIER_MIN_Y, 7, 16, BOTTOM_BARRIER_MAX_Y, 9);
	                    case (ElementPosition.TopLeft):
	                        return new AABB(0, TOP_BARRIER_MIN_Y, 7, 6, TOP_BARRIER_MAX_Y, 9);
	                    case (ElementPosition.TopRight):
	                        return new AABB(10, TOP_BARRIER_MIN_Y, 7, 16, TOP_BARRIER_MAX_Y, 9);
	                    default:
	                        return DEFAULT_AABB;
	                }
	            case (ElementDirection.Z):
	                switch (position)
	                {
	                    case (ElementPosition.BottomRight):
	                        return new AABB(7, BOTTOM_BARRIER_MIN_Y, 0, 9, BOTTOM_BARRIER_MAX_Y, 6);
	                    case (ElementPosition.BottomLeft):
	                        return new AABB(7, BOTTOM_BARRIER_MIN_Y, 10, 9, BOTTOM_BARRIER_MAX_Y, 16);
	                    case (ElementPosition.TopRight):
	                        return new AABB(7, TOP_BARRIER_MIN_Y, 0, 9, TOP_BARRIER_MAX_Y, 6);
	                    case (ElementPosition.TopLeft):
	                        return new AABB(7, TOP_BARRIER_MIN_Y, 10, 9, TOP_BARRIER_MAX_Y, 16);
	                    default:
	                        return DEFAULT_AABB;
	                }
	            default:
	                return DEFAULT_AABB;
	        }
	    }
	
	    internal static AABB Barrier(ElementDirection direction, ElementPosition position)
	    {
	        switch (direction)
	        {
	            case (ElementDirection.X):
	                switch (position)
	                {
	                    case (ElementPosition.Bottom):
	                        return new AABB(0, BOTTOM_BARRIER_MIN_Y, 7, 16, BOTTOM_BARRIER_MAX_Y, 9);
	                    case (ElementPosition.Top):
	                        return new AABB(0, TOP_BARRIER_MIN_Y, 7, 16, TOP_BARRIER_MAX_Y, 9);
	                    default:
	                        return DEFAULT_AABB;
	                }
	            case (ElementDirection.Z):
	                switch (position)
	                {
	                    case (ElementPosition.Bottom):
	                        return new AABB(7, BOTTOM_BARRIER_MIN_Y, 0, 9, BOTTOM_BARRIER_MAX_Y, 16);
	                    case (ElementPosition.Top):
	                        return new AABB(7, TOP_BARRIER_MIN_Y, 0, 9, TOP_BARRIER_MAX_Y, 16);
	                    default:
	                        return DEFAULT_AABB;
	                }
	            default:
	                return DEFAULT_AABB;
	        }
	    }
	
	    internal static AABB AntiJumpOver(ElementDirection direction)
	    {
	        switch (direction)
	        {
	            case (ElementDirection.X):
	                return new AABB(0, 0, 6, 16, 16, 10);
	            case (ElementDirection.Z):
	                return new AABB(6, 0, 0, 10, 16, 16);
	            default:
	                return DEFAULT_AABB;
	        }
	    }
	}
	
	internal partial class FenceSetWizard
	{
	    private bool StepSourceID(string input)
	    {
	        int result = 0;
	        bool success = CommandParser.GetInt(player, input, "block-id", ref result, 1, 1024);
	        SetProps.CopiedFrom = (ushort) result;
	        return success;
	    }
	
	    private bool StepCanJumpOver(string input)
	    {
	        bool result = false;
	        bool success = CommandParser.GetBool(player, input, ref result);
	        SetProps.CanJumpOver = result;
	        return success;
	    }
	
	    private bool StepDoBury(string input)
	    {
	        bool result = false;
	        bool success = CommandParser.GetBool(player, input, ref result);
	        SetProps.DoBury = result;
	        return success;
	    }
	
	    private bool StepCrossIntersect(string input)
	    {
	        bool result = false;
	        bool success = CommandParser.GetBool(player, input, ref result);
	        SetProps.CrossIntersect = result;
	        return success;
	    }
	
	    private bool StepTIntersect(string input)
	    {
	        bool result = false;
	        bool success = CommandParser.GetBool(player, input, ref result);
	        SetProps.TIntersect = result;
	        return success;
	    }
	
	    private bool StepDestID(string input)
	    {
	        int result = 0;
	        bool success = CommandParser.GetInt(player, input, "block-id", ref result, 0, Block.MaxRaw - SetProps.BlocksCount);
	
	        if (success)
	        {
	            if (!IsRangeFree((ushort) result, (ushort)(result + SetProps.BlocksCount - 1), player.Level))
	            {
	                player.Message($"&WThe {result}-{result + SetProps.BlocksCount - 1} range already have level blocks.");
	                player.Message($"&WPlease remove them or choose another range.");
	                success = false;
	            }
	            else
	            {
	                SetProps.CopiedTo = (ushort)result;
	            }
	        }
	
	        return success;
	    }
	}
	
	
	internal static class EasyFencesUtils
	{
	    internal static string ToStringNoComma(this Vec3S32 vector)
	    {
	        return $"{vector.X} {vector.Y} {vector.Z}";
	    }
	}	
	internal class FenceSetProps
	{
	    internal BlockID CopiedFrom     = Block.Wood;
	    internal BlockID CopiedTo       = Block.CPE_MAX_BLOCK + 1;
	    internal bool    CanJumpOver    = true;
	    internal bool    DoBury         = false;
	    internal bool    CrossIntersect = false;
	    internal bool    TIntersect     = false;
	    internal bool    Global         = false;
	
	    internal int BlocksCount {
	        get {
	            int post = 1;
	            int antiJumpOver = CanJumpOver ? 0 : 2;
	            int corners = 8;
	            int barriers = 2;
	
	            if (TIntersect)     corners += 4;
	            if (CrossIntersect) barriers += 1;
	
	            return post + antiJumpOver + corners + barriers;
	        }
	    }
	}
	
	public class EasyFencesPlugin : Plugin
	{
	    public override string name => "EasyFencesPlugin";
	    public override string MCGalaxy_Version => "1.9.4.3";
	
	    public override void Load(bool auto)
	    {
	        Command.Register(new CmdEasyFences());
	    }
	
	    public override void Unload(bool auto)
	    {
	        Command.Unregister(Command.Find("easyfences"));
	    }
	}	
	internal partial class FenceSetWizard
	{
	    private FenceSetProps SetProps;
	
	    private delegate bool Step(string input);
	    private Player player;
	    private int currentStepIndex;
	
	    private string startMsg = "&SYou have started the fence creation process.";
	    private string abortMsg = "&SUse &T/ezf abort &Sat any time to stop making the fences.";
	    private string promptInputMsg = "&SUse &T/ezf [input] &Sto provide input.";
	    private string dashes = "&f--------------------------";
	
	    private List<Step> steps = new List<Step>();
	    private List<string[]> instructions = new List<string[]>();
	
	    private bool IsEnd => currentStepIndex == steps.Count;
	
	    internal FenceSetWizard(Player p)
	    {
	        SetProps = new FenceSetProps();
	        player = p;
	        currentStepIndex = 0;
	
	        MakeSteps();
	        WizardWelcome();
	        CurrentInstructions();
	    }
	
	    private void CurrentInstructions()
	    {
	        string[] currentInstructions = instructions[currentStepIndex];
	
	        for (int i = 0; i < currentInstructions.Length; i++)
	        {
	            player.Message(currentInstructions[i]);
	        }
	
	        player.Message(dashes);
	    }
	
	    private void WizardWelcome()
	    {
	        player.Message(startMsg);
	        player.Message(abortMsg);
	        player.Message(promptInputMsg);
	        player.Message(dashes);
	    }
	
	    private void MakeSteps()
	    {
	        steps = new List<Step>()
	        {
	            StepSourceID,
	            StepDoBury,
	            StepTIntersect,
	            StepCrossIntersect,
	            StepCanJumpOver,
	            StepDestID
	        };
	
	        instructions = new List<string[]>()
	        {
	            instructionsSourceID,
	            instructionsDoBury,
	            instructionsTIntersect,
	            instructionsCrossIntersect,
	            instructionsCanJumpOver,
	            instructionsDestID
	        };
	    }
	
	    internal bool ManageInput(string input)
	    {
	        Step CurrentStep = steps[currentStepIndex];
	
	        if (CurrentStep(input))
	            currentStepIndex += 1;
	
	        if (!IsEnd) CurrentInstructions();
	        return IsEnd;
	    }
	
	    internal List<FenceElement> BuildFenceElements()
	    {
	        List <FenceElement> fenceElements = new List<FenceElement>();
	
	        AddPostElement(fenceElements);
	        AddCornerElements(fenceElements);
	        AddBarrierElements(fenceElements);
	        if (!SetProps.CanJumpOver) AddAntiJumpElements(fenceElements);
	
	        return fenceElements;
	    }
	
	    private void AddPostElement(List<FenceElement> fenceElements)
	    {
	        fenceElements.Add(
	            new FenceElement(
	                type: ElementType.Post,
	                copiedFrom: SetProps.CopiedFrom,
	                copiedTo: SetProps.CopiedTo
	            )
	        );
	    }
	
	    private void AddCornerElements(List<FenceElement> fenceElements)
	    {
	        int offset;
	
	        foreach (ElementPosition position in Enum.GetValues(typeof(ElementPosition)))
	        {
	            foreach (ElementDirection direction in Enum.GetValues(typeof(ElementDirection)))
	            {
	                if (position == ElementPosition.Top || position == ElementPosition.Bottom) continue;
	                offset = GetDefaultOffset(ElementType.Corner, position, direction);
	
	                fenceElements.Add(
	                    new FenceElement(
	                        type: ElementType.Corner,
	                        copiedFrom: SetProps.CopiedFrom,
	                        copiedTo: SetProps.CopiedTo,
	                        direction: direction,
	                        position: position,
	                        offset: AdaptOffset(offset)
	                    )
	                );
	
	                if (SetProps.TIntersect && direction == ElementDirection.X)
	                {
	                    fenceElements.Add(
	                        new FenceElement(
	                            type: ElementType.Corner,
	                            copiedFrom: SetProps.CopiedFrom,
	                            copiedTo: SetProps.CopiedTo,
	                            direction: direction,
	                            position: position,
	                            offset: AdaptOffset(offset + 2)
	                        )
	                    );
	                }
	            }
	        }
	    }
	
	    private int GetDefaultOffset(ElementType type, ElementPosition position, ElementDirection direction)
	    {
	        switch (type)
	        {
	            case ElementType.Corner:
	                switch (direction)
	                {
	                    case ElementDirection.X:
	                        switch (position)
	                        {
	                            case ElementPosition.BottomLeft:
	                            case ElementPosition.BottomRight:
	                                return 1;
	                            case ElementPosition.TopLeft:
	                            case ElementPosition.TopRight:
	                                return 2;
	                            default:
	                                return 0;
	                        }
	                    case ElementDirection.Z:
	                        switch (position)
	                        {
	                            case ElementPosition.BottomLeft:
	                            case ElementPosition.BottomRight:
	                                return 3;
	                            case ElementPosition.TopLeft:
	                            case ElementPosition.TopRight:
	                                return 4;
	                            default:
	                                return 0;
	                        }
	                    default:
	                        return 0; ;
	                }
	            case ElementType.Barrier:
	                switch (position)
	                {
	                    case ElementPosition.Bottom:
	                        return 1;
	                    case ElementPosition.Top:
	                        return 2;
	                    default:
	                        return 0;
	                }
	            default:
	                return 0;
	        }
	    }
	
	    private int AdaptOffset(int offset)
	    {
	        if      (SetProps.DoBury)       return -(offset + 1);
	        else if (!SetProps.CanJumpOver) return offset + 1;
	        return offset;
	    }
	
	    private void AddBarrierElements(List<FenceElement> fenceElements)
	    {
	        int offset;
	
	        foreach (ElementDirection direction in Enum.GetValues(typeof(ElementDirection)))
	        {
	            foreach (ElementPosition position in Enum.GetValues(typeof(ElementPosition)))
	            {
	                if (position == ElementPosition.BottomLeft || position == ElementPosition.BottomRight ||
	                    position == ElementPosition.TopLeft || position == ElementPosition.TopRight) continue;
	
	                offset = GetDefaultOffset(ElementType.Barrier, position, direction);
	
	                fenceElements.Add(
	                    new FenceElement(
	                        type: ElementType.Barrier,
	                        copiedFrom: SetProps.CopiedFrom,
	                        copiedTo: SetProps.CopiedTo,
	                        direction: direction,
	                        position: position,
	                        offset: AdaptOffset(offset)
	                    )
	                );
	
	                if (SetProps.CrossIntersect && direction == ElementDirection.X)
	                {
	                    fenceElements.Add(
	                        new FenceElement(
	                            type: ElementType.Barrier,
	                            copiedFrom: SetProps.CopiedFrom,
	                            copiedTo: SetProps.CopiedTo,
	                            direction: direction,
	                            position: position,
	                            offset: AdaptOffset(offset + 2)
	                        )
	                    );
	                }
	            }
	        }
	    }
	
	    private void AddAntiJumpElements(List<FenceElement> fenceElements)
	    {
	        fenceElements.Add(
	            new FenceElement(
	                type: ElementType.AntiJumpOver,
	                copiedFrom: SetProps.CopiedFrom,
	                copiedTo: SetProps.CopiedTo,
	                direction: ElementDirection.X
	            )
	        );
	
	        fenceElements.Add(
	            new FenceElement(
	                type: ElementType.AntiJumpOver,
	                copiedFrom: SetProps.CopiedFrom,
	                copiedTo: SetProps.CopiedTo,
	                direction: ElementDirection.Z
	            )
	        );
	
	        fenceElements.Add(
	            new FenceElement(
	                type: ElementType.AntiJumpOverCorner,
	                copiedFrom: SetProps.CopiedFrom,
	                copiedTo: SetProps.CopiedTo
	            )
	        );
	    }
	
	    internal bool IsRangeFree(BlockID rawBlockMin, BlockID rawBlockMax, Level level)
	    {
	        BlockDefinition[] defs = level.CustomBlockDefs;
	        BlockID blockMin = Block.FromRaw(rawBlockMin);
	        BlockID blockMax = Block.FromRaw(rawBlockMax);
	
	        for (int b = blockMin; b <= blockMax; b++)
	        {
	            if (defs[b] != null) return false;
	        }
	
	        return true;
	    }
	}
}
