# MiniProject

最小测试模版工程，用于测试各种功能。

- HybridCLR
- UniTask
- Universal RP
- UnityToolkit


## 项目初始化

初始化子模块
```shell
git submodule update --init --recursive
```

安装python3
```shell
sudo apt-get install python3
```

安装python依赖
```shell
pip install -r Tools/requirements.txt
```



## 打包APK
```shell
python3 Tools/build_apk.py
```

## 打包AB
```shell
python3 Tools/build_ab.py
```

## 启动测试文件服务器
```shell
python3 Tools/custom_addressables_host.py
```