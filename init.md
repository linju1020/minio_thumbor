# 安装docker
```bash
sudo curl -fsSL https://get.docker.com | bash -s docker --mirror Aliyun
```

# 安装docker-compose 
```bash
sudo curl -L "https://github.com/docker/compose/releases/download/1.24.1/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose

sudo chmod +x /usr/local/bin/docker-compose

sudo ln -s /usr/local/bin/docker-compose /usr/bin/docker-compose

sudo docker-compose --version
```

# 安装rz（上传）、sz（下载）
```bash
sudo apt-get install lrzsz
```

# 安装私有仓库
## 下载 registry
```bash
sudo docker pull registry
```
## 挂载相关的配置
```bash
sudo mkdir -p  /docker/registry/auth
```
## 生成账号密码：ljg ljg123+++
```bash
sudo docker run --entrypoint htpasswd registry:latest -Bbn ljg ljg123+++ >> /docker/registry/auth/htpasswd
# ERR docker: Error response from daemon: OCI runtime create failed: container_linux.go:349: starting container process caused "exec: \"htpasswd\": executable file not found in $PATH": unknown.
# 解决：https://stackoverflow.com/questions/62531462/docker-local-registry-exec-htpasswd-executable-file-not-found-in-path
sudo htpasswd -Bbn ljg ljg123+++ > /docker/registry/auth/htpasswd
```

## 设置配置文件
```bash
sudo mkdir -p  /docker/registry/config

# 创建以下文件
sudo echo 'version: 0.1
log:
  fields:
    service: registry
storage:
  delete:
    enabled: true
  cache:
    blobdescriptor: inmemory
  filesystem:
    rootdirectory: /var/lib/registry
http:
  addr: :5000
  headers:
    X-Content-Type-Options: [nosniff]
health:
  storagedriver:
    enabled: true
    interval: 10s
threshold: 3' > /docker/registry/config/config.yml
```
## 启动Docker registry
```bash
sudo docker run -d -p 5000:5000 --restart=always  --name=registry \
-v /docker/registry/config/:/etc/docker/registry/ \
-v /docker/registry/auth/:/auth/ \
-e "REGISTRY_AUTH=htpasswd" \
-e "REGISTRY_AUTH_HTPASSWD_REALM=Registry Realm" \
-e REGISTRY_AUTH_HTPASSWD_PATH=/auth/htpasswd \
-v /docker/registry/:/var/lib/registry/ \
registry
```

## 配置Windows客户端
```bash
#1、登录搭建的私有docker仓库 *.*.*.*是服务器IP地址
sudo docker login *.*.*.*:5000
#输入用户名密码，这个地方会报错，解决办法把*.*.*.*:5000加入到configuration file中的Insecure registries（在docker客户端中直接操作）
#Error response from daemon: Get https://*.*.*.*:5000/v2/: http: server gave HTTP response to HTTPS client
```
## 配置Linux客户端
```bash
# 1、由于docker的版本从13开始，register只能用https，由于我们配的都是http的，所以要先在/etc/docker/目录创建daemon.json文件并加入如下
sudo echo '{"insecure-registries":["*.*.*.*:5000"]}' > /etc/docker/daemon.json

sudo systemctl restart docker

sudo docker login *.*.*.*:5000
```