# 微信认证中心

This project was deprecated!

## 调试

&emsp;&emsp;调试需要安装[微信开发者工具](https://developers.weixin.qq.com/miniprogram/dev/devtools/download.html)。请联系公众号运营方登记为开发者、关注测试号，然后开始启动调试。

调试时，在开发者工具中，构造如下的链接来发起调用：

``` uri
http://127.0.0.1:15335/Authorize?wxappid=wx65c5ac17e0ec33cb&clientid=28b6e6ad-b1f0-4c91-9be1-fbba55f5af66&resource=http%3A%2F%2Fwebapi.changingsoft.com%2Fadfs%2Fservices%2Ftrust&redirect_uri=https%3A%2F%2Fatop-wx-test.changingsoft.com%2FCallBack
```
