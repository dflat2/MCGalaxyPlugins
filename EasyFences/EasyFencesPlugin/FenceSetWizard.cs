namespace PluginEasyFences;
using System;
using System.Collections.Generic;
using MCGalaxy;
using BlockID = System.UInt16;

public partial class FenceSetWizard {
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

    public FenceSetWizard(Player p) {
        SetProps = new FenceSetProps();
        player = p;
        currentStepIndex = 0;

        MakeSteps();
        WizardWelcome();
        CurrentInstructions();
    }

    private void CurrentInstructions() {
        string[] currentInstructions = instructions[currentStepIndex];

        for (int i = 0; i < currentInstructions.Length; i++) {
            player.Message(currentInstructions[i]);
        }

        player.Message(dashes);
    }

    private void WizardWelcome() {
        player.Message(startMsg);
        player.Message(abortMsg);
        player.Message(promptInputMsg);
        player.Message(dashes);
    }

    private void MakeSteps() {
        steps = new List<Step>() {
            StepSourceID,
            StepDoBury,
            StepTIntersect,
            StepCrossIntersect,
            StepCanJumpOver,
            StepDestID
        };

        instructions = new List<string[]>() {
            instructionsSourceID,
            instructionsDoBury,
            instructionsTIntersect,
            instructionsCrossIntersect,
            instructionsCanJumpOver,
            instructionsDestID
        };
    }

    public bool ManageInput(string input) {
        Step CurrentStep = steps[currentStepIndex];

        if (CurrentStep(input))
            currentStepIndex += 1;

        if (!IsEnd) CurrentInstructions();
        return IsEnd;
    }

    public List<FenceElement> BuildFenceElements() {
        List <FenceElement> fenceElements = new List<FenceElement>();

        AddPostElement(fenceElements);
        AddCornerElements(fenceElements);
        AddBarrierElements(fenceElements);
        if (!SetProps.CanJumpOver) AddAntiJumpElements(fenceElements);

        return fenceElements;
    }

    private void AddPostElement(List<FenceElement> fenceElements) {
        fenceElements.Add(
            new FenceElement(
                type: ElementType.Post,
                copiedFrom: SetProps.CopiedFrom,
                copiedTo: SetProps.CopiedTo
            )
        );
    }

    private void AddCornerElements(List<FenceElement> fenceElements) {
        int offset;

        foreach (ElementPosition position in Enum.GetValues(typeof(ElementPosition))) {
            foreach (ElementDirection direction in Enum.GetValues(typeof(ElementDirection))) {
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

                if (SetProps.TIntersect && direction == ElementDirection.X) {
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

    private int GetDefaultOffset(ElementType type, ElementPosition position, ElementDirection direction) {
        switch (type) {
            case ElementType.Corner:
                switch (direction) {
                    case ElementDirection.X:
                        switch (position) {
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
                        switch (position) {
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
                switch (position) {
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

    private int AdaptOffset(int offset) {
        if (SetProps.DoBury) {
            return -(offset + 1);
        } else if (!SetProps.CanJumpOver) {
            return offset + 1;
        }

        return offset;
    }

    private void AddBarrierElements(List<FenceElement> fenceElements) {
        int offset;

        foreach (ElementDirection direction in Enum.GetValues(typeof(ElementDirection))) {
            foreach (ElementPosition position in Enum.GetValues(typeof(ElementPosition))) {
                if (position == ElementPosition.BottomLeft || position == ElementPosition.BottomRight || position == ElementPosition.TopLeft || position == ElementPosition.TopRight) {
                    continue;
                }

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

                if (SetProps.CrossIntersect && direction == ElementDirection.X) {
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

    private void AddAntiJumpElements(List<FenceElement> fenceElements) {
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

    public bool IsRangeFree(BlockID rawBlockMin, BlockID rawBlockMax, Level level) {
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