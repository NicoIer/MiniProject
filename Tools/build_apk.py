import os
import shutil

'''
Using For Unity Project
'''


def cp_hot_update_dll(platform: str, src_dir: str, tar_dir: str, hybrid_assembly_list: list[str]):
    src_dir = os.path.abspath(src_dir)
    src_dir = os.path.join(src_dir, platform + "/")

    tar_dir = os.path.abspath(tar_dir)
    tar_dir = os.path.join(tar_dir, platform + "/")

    # # 移除目标文件夹下的所有非.meta文件
    # for file in os.listdir(tar_dir):
    #     if not file.endswith(".meta"):
    #         os.remove(os.path.join(tar_dir, file))

    # 创建目标文件夹

    # 拷贝dll文件
    for assembly in hybrid_assembly_list:
        # 复制文件到目标文件夹
        src_file = os.path.join(src_dir, assembly + ".dll")
        tar_file = os.path.join(tar_dir, assembly + ".dll.bytes")
        print("复制文件: " + src_file + " -> " + tar_file)
        # 目标文件不存在 则创建
        if not os.path.exists(tar_file):
            open(tar_file, 'w').close()

        # 覆盖模式进行复制
        shutil.copyfile(src_file, tar_file,True)


# 构建整个项目
if __name__ == "__main__":
    print("python执行的路径: " + __file__)

    platform = "Android"

    hybrid_dll_source_dir = "../HybridCLRData/HotUpdateDlls/"
    hybrid_dll_target_path = "../Assets/Resource/HybridCLR/HotUpdateDlls/"

    hybrid_assembly_list = [
        "Game"
    ]

    cp_hot_update_dll(platform, os.path.abspath(hybrid_dll_source_dir), os.path.abspath(hybrid_dll_target_path),
                      hybrid_assembly_list)
