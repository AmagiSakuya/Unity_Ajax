# Unity Ajax

自己封装的Unity HTTP请求接口

依赖第三方插件 LitJson 和 RSGPromise 

亲测PC/Android没有问题，理论上全平台通用

POST请求暂未支持流文件上传

> *注意：须在mono脚本调用

## 简单使用例
```Csharp
// # this为继承MonoBehaviour的脚本
// # sendData为任意对象变量
// # resultData为后台返回值转C#对象的类型(Type) ,可以为数组 如 resultData[] 接收数组开始的json

// Get请求
Ajax.Get<resultData>(this, "url", sendData).Then((data) =>
{
	Debug.Log(data);
}).Catch(err => {
	Debug.Log(err.ToString());
});

// Post请求
Ajax.Post<resultData>(this, "url", sendData).Then((data) =>
{
	Debug.Log(data);
}).Catch(err => {
	Debug.Log(err.ToString());
});

// Put请求
Ajax.Put<resultData>(this, "url", sendData).Then((data) =>
{
	Debug.Log(data);
}).Catch(err => {
	Debug.Log(err.ToString());
});

// Delete请求
Ajax.Delete<resultData>(this, "url", sendData).Then((data) =>
{
	Debug.Log(data);
}).Catch(err => {
	Debug.Log(err.ToString());
});

// #RequestType为枚举类型 本例子使用Get
// 请求 
Ajax.Request<resultData>(this, "url",RequestType.GET, sendData).Then((data) =>
{
	Debug.Log(data);
}).Catch(err => {
	Debug.Log(err.ToString());
});

```

## 接口参数
| 参数        | 类型    |  描述  | 必须  |
| --------   | -----  | ---- | ---- |
| T        | T     |   后台返回值转C#对象的类型(Type),可以为对象类型、string、LitJson。     | 是 |
| mono        | MonoBehaviour      |   mono脚本 在Mono上调用时一般传入this即可    | 是 |
| url        | string      |   请求地址    | 是 |
| sendData        | object      |   请求数据的对象    | 否 |
| headers        |  Dictionary<string, string>      |   请求头    | 否 |