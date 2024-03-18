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
    current_line = clean_string_ascii(current_line)

    if current_line.startswith("namespace"):
        return
    if current_line.startswith("using"):
        usings.append(current_line)
        return

    if only_spaces(current_line):
        current_line = "\n"
    
    if current_line != "\n":
        current_line = prepend_tab(current_line)

    lines.append(current_line)

def only_spaces(string):
    for character in string:
        if character != " " and character != "\t" and character != "\n":
            return False

    return True

def prepend_tab(text):
    return '    ' + text

def clean_string_ascii(text):
    chars = list()

    for c in text:
        if ord(c) <= 255:
            chars.append(c)
    
    return "".join(chars)

def remove_doubles(a_list):
    return list(set(a_list))

def build_merged_file_content(plugin_name, lines, usings):
    EMPTY_LINE = "\n"
    file_lines = list()

    for using in usings:
        file_lines.append(using)

    file_lines.append(EMPTY_LINE)
    file_lines.append("namespace " + plugin_name + "Plugin\n")
    file_lines.append("{")

    for line in lines:
        file_lines.append(line)

    file_lines.append("\n}")
    return "".join(file_lines)

if __name__ == "__main__":
    main(sys.argv)
