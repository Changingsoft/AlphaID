# Alpha ID 样例数据

使用DatabaseTool创建数据库将会创建部分样例数据，以供开发调试和集成回归测试时使用。

## 自然人

|Id|Name|UserName（登录名）|Email|Phone Number|Password|备注|
|---|---|---|---|---|---|---|
|`d2480421-8a15-4292-8e8f-06985a1f645b`|刘备|liubei|liubei@sanguo.net|+8613812340001|Pass123$||
|`bf16436b-d15f-44b7-bd61-831eacee5063`|关羽|guanyu|guanyu@sanguo.net|+8613812340002|Pass123$||
|`c12c61e6-49a3-4f6d-8cea-7f039ec371a1`|张飞|zhangfei|zhangfei@sanguo.net|+8613812340003|Pass123$||
|`f23eef91-e089-4164-99a7-909cdae5eac7`|诸葛亮|zhugeliang|zhugeliang@sanguo.net|+8613812340004|Pass123$||
|`020d87bb-a337-457c-af6d-53bc2d355579`|曹操|caocao|caocao@sanguo.net||Pass123$||

## 组织

|ID|名称|统一社会信用代码|住所|负责人|备注|
|---|---|---|---|---|---|
|`1c86b543-0c92-4cd8-bcd5-b4e462847e59`|北魏集团||洛阳|曹操||
|`5288b813-e1f4-4fd3-a342-6f21a4c3fef7`|东吴集团||建业|孙权||
|`a7be43af-8b49-450e-a600-90a8748e48a5`|蜀汉集团||成都|刘备||
|`c50f590b-5757-4e64-aaf8-0c924dc4e912`|曹丞相府管理有限公司||四川省成都市浆洗街28号|||


## 组织关系

|组织|个人|职务（Title）|部门|备注|特性|可见性|
|---|---|---|---|---|---|---|
|汉王大业发展集团|刘备|总裁|||创建者（Owner）|Public|
|北魏集团|曹操|总裁|||创建者（Owner）|Public|
