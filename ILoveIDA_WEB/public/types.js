export const list = [{
    Name: "COTP连接报文",
    Configs: [{ Index: 1, Value: "测试", Type: "Prop", Len: 2 },
    { Index: 2, Value: "0x83|M区,0x84|DB区", Type: "Sel", Len: 2 },
    { Index: 3, Value: "SessionId", Type: "Val", Len: 2 },
    { Index: 4, Value: "SendCount", Type: "Val", Len: 2 },
    { Index: 5, Value: "DataCount", Type: "Val", Len: 2 }
    ],
    List: [
        {
            "Name": "版本号",
            "Value": "0x03"
        },
        {
            "Name": "保留位",
            "Value": "0x00"
        },
        {
            "Value": "0x00",
            "Name": "长度"
        },
        {
            "Value": "0x16",
            "Name": "长度"
        },
        {
            "Value": "0x11",
            "Name": "字节数"
        },
        {
            "Value": "0xe0",
            "Name": "PDU，请求连接"
        },
        {
            "Value": "0x00",
            "Name": "目标引用"
        },
        {
            "Value": "0x00",
            "Name": "目标引用"
        },
        {
            "Value": "0x00",
            "Name": "源引用"
        },
        {
            "Value": "0x2e",
            "Name": "源引用"
        },
        {
            "Value": "0x00",
            "Name": "扩展格式"
        },
        {
            "Value": "0xc1",
            "Name": "源上位机"
        },
        {
            "Value": "0x02",
            "Name": "参数长度"
        },
        {
            "Value": "0x10",
            "Name": "双边通讯"
        },
        //Local
        {
            "Value": "0x01",
            "Name": "机架号"
        },
        //Local
        {
            "Value": "0x00",
            "Name": "目的，PLC"
        },
        {
            "Value": "0x02",
            "Name": "参数长度"
        },
        {
            "Value": "0x03",
            "Name": "s7单边模式"
        },
        //Remote
        {
            "Value": "0x03",
            "Name": "机架号"
        },
        //Remote
        {
            "Value": "0x00",
            "Name": "参数代码"
        },
        {
            "Value": "0x01",
            "Name": "参数长度"
        },
        {
            "Value": "0x0a",
            "Name": "数据单元"
        }
    ]
},
{
    Name: "S7连接报文",
    List: [
        {
            "Value": "0x03"
        },
        {
            "Value": "0x00"
        },
        {
            "Value": "0x00"
        },
        {
            "Value": "0x19"
        },
        {
            "Value": "0x02"
        },
        {
            "Value": "0xf0"
        },
        {
            "Value": "0x80"
        },
        {
            "Value": "0x32"
        },
        {
            "Value": "0x01"
        },
        {
            "Value": "0x00"
        },
        {
            "Value": "0x00"
        },
        {
            "Value": "0x03"
        },
        {
            "Value": "0x7c"
        },
        {
            "Value": "0x00"
        },
        {
            "Value": "0x08"
        },
        {
            "Value": "0x00"
        },
        {
            "Value": "0x00"
        },
        {
            "Value": "0xf0"
        },
        {
            "Value": "0x00"
        },
        {
            "Value": "0x00"
        },
        {
            "Value": "0x01"
        },
        {
            "Value": "0x00"
        },
        {
            "Value": "0x01"
        },
        {
            "Value": "0x03"
        },
        {
            "Value": "0xc0"
        }
    ]
}, {
    Name: "读取报文",
    List: [
        {
            "Value": "0x03"
        },
        {
            "Value": "0x00"
        },
        {
            "Value": "0x00"
        },
        {
            "Value": "0x1f"
        },
        {
            "Value": "0x02"
        },
        {
            "Value": "0xf0"
        },
        {
            "Value": "0x80"
        },
        {
            "Value": "0x32"
        },
        {
            "Value": "0x01"
        },
        {
            "Value": "0x00"
        },
        {
            "Value": "0x00"
        },
        {
            "Value": "0x00"
        },
        {
            "Value": "0x00"
        },
        {
            "Value": "0x00"
        },
        {
            "Value": "0x0e"
        },
        {
            "Value": "0x00"
        },
        {
            "Value": "0x00"
        },
        {
            "Value": "0x04"
        },
        {
            "Value": "0x01"
        },
        {
            "Value": "0x12"
        },
        {
            "Value": "0x0a"
        },
        {
            "Value": "0x10"
        },
        {
            "Value": "0x02"
        },
        {
            "Value": "0x00"
        },
        {
            "Value": "0x01"
        },
        {
            "Value": "0x00"
        },
        {
            "Value": "0x00"
        },
        {
            "Value": "0x83"
        },
        {
            "Value": "0x00"
        },
        {
            "Value": "0x03"
        },
        {
            "Value": "0x20"
        }
    ]
}];