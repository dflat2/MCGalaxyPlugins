# Merges multiple .cs files into one, so that the plugin can easily be loaded
# into MCGalaxy and /pcompile'd. It won't work if a plugin uses multiple namespaces.
# Moreover, the plugin has to comply with the directory hierarchy of this repo.
# It won't work either with the old namespace notations "namespace A.B { ... }".
# Use "namespace A.B;" instead.
import sys
import glob

def main(args):
    if len(args) != 2:
        print("merge_cs_files: usage: python3 merge_cs_files <plugin_name>")
        exit(1)
    
    _, plugin_name = args

    cs_files_paths = glob.glob("{}/{}Plugin/*.cs".format(plugin_name, plugin_name))

    usings = list()
    lines = list()
    
    for cs_file_path in cs_files_paths:
        process_cs_file(cs_file_path, usings, lines)

    usings = remove_doubles(usings)
    merged_file_content = build_merged_file_content(plugin_name, lines, usings)

    with open("{}/{}.cs".format(plugin_name, plugin_name), "w") as file:
        file.write(merged_file_content)

def process_cs_file(file_path, usings, lines):
    with open(file_path) as file:
        file_lines = file.readlines()

        for current_line in file_lines:
            process_line(current_line, usings, lines)

def process_line(current_line, usings, lines):
    # Sometimes Visual Studio seems to insert weird characters that I have to remove
    # notably U+FEFF (otherwise it breaks the namespace test). TODO investigate this
    # issue.
    current_line = clean_string_ascii(current_line)

    if current_line.startswith("namespace"):
        return
    if current_line.startswith("using"):
        usings.append(current_line)
        return
    
    lines.append(current_line)

def clean_string_ascii(text):
    chars = list()

    for c in text:
        if ord(c) <= 255:
            chars.append(c)
    
    return "".join(chars)

def remove_doubles(a_list):
    return list(set(a_list))

def build_merged_file_content(plugin_name, lines, usings):
    file_lines = ["namespace {}Plugin ".format(plugin_name)]
    file_lines.append("{\n")
    
    for using in usings:
        file_lines.append(using)

    for line in lines:
        file_lines.append(line)

    file_lines.append("}")
    return "".join(file_lines)


if __name__ == "__main__":
    main(sys.argv)
