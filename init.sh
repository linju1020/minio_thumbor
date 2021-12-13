#安装docker
sudo curl -fsSL https://get.docker.com | bash -s docker --mirror Aliyun

#安装docker-compose 
sudo curl -L "https://github.com/docker/compose/releases/download/1.24.1/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose
sudo ln -s /usr/local/bin/docker-compose /usr/bin/docker-compose
sudo docker-compose --version

#安装rz（上传）、sz（下载）
sudo apt-get install lrzsz