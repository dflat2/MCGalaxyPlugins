import sys
import os
import shutil

plugin_name_placeholder = "__PLUGINNAME__"

def main(args):
    if len(args) != 2:
        print("new_plugin: usage: python newplugin.py <plugin_name>.")
        exit(1)

    _, plugin_name = args

    if (already_exists(plugin_name)):
        print("new_plugin: there is already a plugin with that name.")
        exit(1)

    copy_template_folder(to=plugin_name)
    replace_occurrences_of_template(with_name=plugin_name)
    replace_occurrences_of_placeholder(with_name=plugin_name)

def copy_template_folder(to):
    template_dir = "Template"
    if os.path.exists(template_dir):
        shutil.copytree(template_dir, to)
        print(f"new_plugin: Template copied to {to}.")
    else:
        print("new_plugin: Template folder not found.")
        exit(1)

def replace_occurrences_of_template(with_name):
    for root, dirs, files in os.walk(with_name, topdown=False):
        for file in files:
            if "Template" in file:
                new_name = file.replace("Template", with_name)
                os.rename(os.path.join(root, file), os.path.join(root, new_name))
        for dir in dirs:
            if "Template" in dir:
                new_name = dir.replace("Template", with_name)
                os.rename(os.path.join(root, dir), os.path.join(root, new_name))

def replace_occurrences_of_placeholder(with_name):
    for root, _, files in os.walk(with_name):
        for file in files:
            if file.startswith("."): continue
            file_path = os.path.join(root, file)
            with open(file_path, "r") as f:
                content = f.read()
            updated_content = content.replace(plugin_name_placeholder, with_name)
            with open(file_path, "w") as f:
                f.write(updated_content)

def already_exists(plugin_name):
    return os.path.exists(plugin_name)

if __name__ == "__main__":
    main(sys.argv)
