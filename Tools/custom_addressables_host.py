import os


if __name__ ==  "__main__":
    host_dir = "../ServerData/Android/"
    host_dir = os.path.abspath(host_dir)

    port = 9876
    # 启动http服务器在指定目录 允许文件下载
    host_str = str.format("python3 -m http.server {} --directory {}",port,host_dir)

    os.system(host_str)
