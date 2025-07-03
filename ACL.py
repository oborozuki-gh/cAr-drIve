import os
import subprocess

file_name = "LearnSetting.txt"

model_name = None
num_run = None
start_id = None
env = None
config_file = None
log_files = []
if os.path.isfile(file_name):
    with open(file_name, "r", encoding="utf-8") as rf:
        for line in rf:
            line = line.replace(" ", "").replace("\n", "")
            if "#" in line:
                continue
            if "model_name" in line:
                model_name = line.replace("model_name:", "")

            elif "num_run" in line:
                num_run = int(line.replace("num_run:", ""))

            elif "start_id" in line:
                start_id = int(line.replace("start_id:", ""))

            elif "log_files" in line:
                log_files = line.replace("log_files:", "").split(",")

            elif "env" in line:
                env = line.replace("env:", "")

            elif "config_file" in line:
                config_file = line.replace("config_file:", "")

if model_name == None or model_name == "":
    model_name = input("Model name : ")

if num_run == None:
    num_run = int(input("Number of times Leaning : "))

if start_id == None:
    start_id = int(input("Start id : "))

if env == None:
    env = input("Environment name : ")

if config_file == None:
    config_file = input("Config file name : ")

def Learning(command):
    with subprocess.Popen(command, shell=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=True) as process:
        try:
            for line in process.stdout:
                print(line, end='')
            for line in process.stderr:
                print(line, end='')
        except Exception as e:
            print(f"An error occurred: {e}")

for i in range(start_id, start_id+num_run):
    run_id = model_name + str(i).zfill(3)
    command = f"mlagents-learn {config_file} --env={env} --width=720 --height=480 --no-graphics --run-id={run_id}"
    # command = f"mlagents-learn {config_file} --run-id={run_id}"
    if os.path.isdir(f"./results/{run_id}"):
        print(f"Already have a {run_id} model.")
        continue
    #     subprocess.run(f"rd /s /q results\{run_id}", shell=True)
    print(f"Start Learning {run_id}")
    Learning(command)
    for log_file in log_files:
        if os.path.isfile(log_file):
            subprocess.run(f"move {log_file} --num-envs=10 results\{run_id}\\", shell=True)
    if os.path.isfile("settingParameters.txt"):
        subprocess.run(f"move settingParameters.txt results\{run_id}\\", shell=True)
    subprocess.run(f"ren results\{run_id}\CarDrive.onnx {run_id}.onnx", shell=True)
    if os.path.isdir("./Assets/NN Models"):
        subprocess.run(f'copy /Y results\{run_id}\{run_id}.onnx "Assets\\NN Models"', shell=True)
