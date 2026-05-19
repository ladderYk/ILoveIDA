<template>
    <el-row :gutter="15" style="height: 100%;">

        <el-col :span="4" style="height: 100%;">
            <div class="flex">
                <span style="font-size: 18px;">
                    报文列表
                </span>
                <el-button size="small" @click="addType" type="primary" :icon="Plus">添加报文</el-button>
            </div>
            <el-card shadow="never" style="height: calc(100% - 40px);">
                <el-tree style="max-width: 600px" :data="list" :props="defaultProps" @node-click="handleNodeClick" />
            </el-card>
        </el-col>
        <el-col :span="20" style="height: 100%;">
            <el-space style="margin-bottom: 10px;">
                <el-input v-model="Prots.Name" placeholder="报文名称" style="width: 200px" />
                <el-button size="small" :icon="Plus" :onclick="onAddItem1">添加</el-button>
                <el-button size="small" :icon="Refresh">清空</el-button>
                <el-button size="small" :icon="Refresh" :onclick="onExpItem1">导出</el-button>
                <el-button size="small" :icon="Refresh" :onclick="onExpItem2">生成</el-button>
                <el-button :onclick="showConfigs">参数({{ Prots.Configs.length }})</el-button>
                <el-button size="small" :icon="Plus" :onclick="onTest">测试</el-button>
            </el-space>

            <el-scrollbar v-if="Prots.List.length > 0" style="height: calc(100% - 60px)" aria-orientation="vertical">
                <div style="overflow-x: hidden;">
                    <el-row>
                        <template v-for="(data, i) in Prots.List">
                            <el-col :span="3">
                                <el-card shadow="hover" body-style="padding: 15px">
                                    <div class="flex">
                                        <span>
                                            {{ i + 1 }}
                                        </span>
                                        <el-button size="small" :icon="Close" title="删除"
                                            @click.prevent="deleteRow1(i)" />
                                    </div>
                                    <el-input v-model="data[0]" placeholder="值" />
                                    <el-input v-model="data[1]" placeholder="说明" />
                                </el-card>
                            </el-col>
                            <el-col :span="3" v-if="Prots.Configs.find(c => c.Index == i + 1)">
                                <el-card shadow="hover" body-style="padding: 15px">
                                    <div class="flex">
                                        <span>
                                            {{Prots.Configs.find(c => c.Index == i + 1).Name}}
                                        </span>
                                        <el-button size="small" :icon="Close" title="删除" />
                                    </div>
                                    <el-input placeholder="值" />
                                    <el-input v-model="Prots.Configs.find(c => c.Index == i + 1).Type"
                                        placeholder="类型" />
                                </el-card>
                            </el-col>
                        </template>
                        <template v-for="(data, i) in Prots.Configs">
                            <el-col :span="3" v-if="data.Index > Prots.List.length">
                                <el-card shadow="hover" body-style="padding: 15px" body-class="hasParam">
                                    <div class="flex">
                                        <span>
                                            {{ data.Name }}
                                        </span>
                                        <el-button size="small" :icon="Close" title="删除" />
                                    </div>
                                    <el-input placeholder="值" />
                                    <el-input v-model="data.Type" placeholder="类型" />
                                </el-card>
                            </el-col>
                        </template>
                        <!-- <el-col :span="3" v-for="(data, i) in Prots.List">
                            <el-card shadow="hover" body-style="padding: 15px"
                                :class="Prots.Configs.find(c => c.Index == i + 1) ? 'hasParam' : ''">
                                <div class="flex">
                                    <span>
                                        {{ i + 1 }}
                                    </span>
                                    <el-button size="small" :icon="Close" title="删除" @click.prevent="deleteRow1(i)" />
                                </div>
                                <el-input v-model="data[0]" placeholder="值" />
                                <el-input v-model="data[1]" placeholder="说明" />
                            </el-card>
                        </el-col> -->
                        <el-col :span="3">
                            <el-card style="margin-bottom: 10px;height: 134px;" shadow="hover"
                                body-style="padding: 15px;text-align:center;">
                                <el-button size="large" :icon="Plus" :onclick="onAddItem1">添加</el-button>
                            </el-card>
                        </el-col>
                    </el-row>
                </div>
            </el-scrollbar>
        </el-col>
    </el-row>
    <el-dialog v-model="dialogFormVisible" title="参数配置" width="800">
        <el-button size="small" :icon="Plus" :onclick="onAddItem2">添加</el-button>
        <el-table :data="Prots.Configs" table-layout="auto">
            <el-table-column label="名称">
                <template #default="scope">
                    <el-input v-model="scope.row.Name" autocomplete="off" />
                </template>
            </el-table-column>
            <el-table-column label="下标">
                <template #default="scope">
                    <el-input-number v-model="scope.row.Index" autocomplete="off" min="0" style="width: 120px" />
                </template>
            </el-table-column>
            <el-table-column label="类型" width="150">
                <template #default="scope">
                    <!-- <el-select placeholder="请选择" v-model="scope.row.Type">
                        <el-option label="默认" value="" />
                        <el-option label="参数" value="Prop" />
                        <el-option label="下拉选择" value="Sel" />
                        <el-option label="系统变量" value="Val" />
                    </el-select> -->
                    <el-input v-model="scope.row.Type" autocomplete="off" />
                </template>
            </el-table-column>
            <el-table-column label="值">
                <template #default="scope">
                    <el-input v-model="scope.row.Value" autocomplete="off" />
                </template>
            </el-table-column>
            <el-table-column label="操作">
                <template #default="scope">
                    <el-button size="small" :icon="Close" @click.prevent="deleteRow2(scope.$index)" />
                </template>
            </el-table-column>
        </el-table>

    </el-dialog>
</template>
<script setup>

// 输入参数，名称，手动填写、下拉、选择变量，位数。
// TODO 两个接口
// 编辑/修改，禁用/恢复
import { onMounted, ref, reactive, computed, watch } from "vue";
import { Close, Link, Plus, Refresh } from '@element-plus/icons-vue'
import { post, get } from "../utils/request";
// import { list } from "/public/types.js";
const props = defineProps(['list'])

let sokect = null;
const showEdit = ref(false)

const formRef = ref();
const dialogFormVisible = ref(false);
const tableData = ref([]);
const list = ref(props.list);
watch(() => props.list, (newVal, oldVal) => {
    list.value = newVal;
    Prots.List = [];
    Prots.Configs = [];
});
const rowIndex = ref(-1);
const rowID = ref("");
const form = reactive({
    id: "",
    name: "",
    model: "",
    ip: "",
    port: 102,
    timeout: 0,
    cycle: 0
});
const typeForm = reactive({
    Index: -1,
    Value: "",
    Type: 0,
    Len: 0
});
const ValueLs = computed({
    get() {
        return typeForm.Value.split(',').join('\n');
    },
    set(newVal) {
        typeForm.Value = newVal.split('\n').join(',');
    }
});

const showConfig = (data, i) => {
    dialogFormVisible.value = true;
    var prop = Prots.Configs.find(v => v.Index == i);
    if (prop) {
        typeForm.Type = data.Type ? data.Type : 0;
        typeForm.Value = prop.Value;
        typeForm.Index = prop.Index;
        typeForm.Len = prop.Len;
    } else {
        typeForm.Type = data.Type ? data.Type : 0;
        typeForm.Value = "";
        typeForm.Index = -1;
        typeForm.Len = 0;
    }
}
const showConfigs = (data, i) => {
    dialogFormVisible.value = true;

}
const onSetData = (i, data, cb) => {
    if (data.Type == 1) {
        var prop = Prots.Configs.find(v => v.Index == i);
        data.Value = "Prop";
        data.Type = 0;
    } else {
        Prots.Configs.push({ Index: i });
        data.Value = "Prop";
        data.Type = 1;
    }
    cb();
}
const defaultProps = {
    children: 'children',
    label: 'Name',
}
/**
 * COTP根据plc类型，rack slot
 * 200: 0x10 0x00 | 0x10 0x01
 * Logo0BA8: 0x01 0x00 | 0x02 0x00
 * S7200Smart
 * S71200:
 * S71500:
 * S7300:
 * S7400:
 * 0x01 0x00 | 0x03 (rack<<5)|slot
 */

const modbusTcp = [
    {
        "Value": "0x00"
    },
    {
        "Name": "",
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
        "Value": "0x06"
    },
    {
        "Value": "0x01"
    },
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
        "Value": "0x00"
    },
    {
        "Value": "0x02"
    }
];
const Prots = reactive({ Name: "", Configs: [], List: [] });

const onAddItem2 = () => {
    Prots.Configs.push({});
}
const onTest = () => {
    Prots.List = modbusTcp;
}
const options = ref([]);
const handleNodeClick = (data) => {
    var d = { ...data };
    Prots.Name = d.Name;
    var List = d.Datas?.map(d => { return { ...d } });
    var Configs = d.Params?.map(d => { return { ...d } });
    Prots.List = List ? [...List] : [];
    Prots.Configs = Configs ? [...Configs] : [];
}
const addType = () => {
    Prots.Name = "";
    Prots.List = [];
    Prots.Configs = [];
}
onMounted(() => {
    // get("/DeviceList").then(data => {
    //     tableData.value = data;
    // });
    fetch("/AGV1.json").then(v => v.json()).then(data => {
        //tableData.value = data;
    });
    fetch("/dType.json").then(v => v.json()).then(data => {
        // options.value = data;
    });
    fetch("/Config.json").then(v => v.json()).then(data => {
        //list.value = data[0]["SendProts"];        //options.value = data;
    });
    // get("/TypeList").then(data => {
    //     options.value = data;
    // });

    sokect = new WebSocket(ws);
    sokect.onopen = () => {
        sokect.send(JSON.stringify({ topic: 'online', action: 'subscribe' }));

        //sokect.send(JSON.stringify({ action: 'subscribe', data: 'agvList' }));
    };
    sokect.onerror = () => {
        // setTimeout(() => {
        //   document.location.reload();
        // }, 10000);
    }
    sokect.onmessage = (message) => {
        const jmessage = JSON.parse(message.data);
        var agv = tableData.value.find((v, index, obj) => v.Name == jmessage.name);
        if (agv == null)
            return;
        if (agv.Online != jmessage.online) {
            agv.Online = jmessage.online;
        }
    };
});
const deleteRow1 = (index) => {
    Prots.List.splice(index, 1);
    // var configIdx = Prots.Configs.findIndex(config => config.Index == index);
    // Prots.Configs.splice(configIdx, 1);
    // Prots.Configs.forEach((config, i) => {
    //     if (config.Index > index) {
    //         config.Index--;
    //     }
    // });

}
const deleteRow2 = (index) => {
    Prots.Configs.splice(index, 1);
    // var configIdx = Prots.Configs.findIndex(config => config.Index == index);
    // Prots.Configs.splice(configIdx, 1);
    // Prots.Configs.forEach((config, i) => {
    //     if (config.Index > index) {
    //         config.Index--;
    //     }
    // });

}
const onAddItem1 = () => {
    Prots.List.push({});
}
const onExpItem1 = () => {
    console.log(Prots.List);
}
const onExpItem2 = () => {
    var lis = Prots.List.map(({ Value }) => Value);
    for (var i = 0; i < Prots.Configs.length; i++) {
        lis.splice(Prots.Configs[i].Index + i, 0, Prots.Configs[i].Value);
    }
    console.log(lis);
    console.log(Prots.List.map(({ Value }) => Value).join(" "));
}
const getData = (row) => {
    fetch("http://127.0.0.1/AGVList").then(d => console.log(d.json()));
}
const edit = (index, val) => {
    rowIndex.value = index;
    setForm(val);
}
const editN = (val) => {
    rowID.value = val.ID;
    setForm(val);
}
const setForm = (val) => {
    if (val) {
        form.id = val.ID;
        form.name = val.Name;
        form.model = val.Model;
        form.ip = val.IP;
        form.port = val.Port;
        form.timeout = val.Timeout;
        form.cycle = val.Cycle;
    } else {
        form.id = "";
        form.name = "";
        form.model = "";
        form.ip = "";
        form.port = 102;
        form.timeout = 0;
        form.cycle = 0;
    }
}
const submitForm = (formEl) => {
    if (!formEl) return
    formEl.validate((valid) => {
        if (valid) {
            if (rowIndex.value == -1) {
                tableData.value.push({ ...form });
            } else {
                var indexData = tableData.value[rowIndex.value];
                indexData.name = form.name;
                indexData.model = form.model;
                indexData.ip = form.ip;
                indexData.port = form.port;
                indexData.timeout = form.timeout;
                indexData.cycle = form.cycle;
            }
            resetForm();
        } else {
            return false
        }
    })
}
const resetForm = () => {
    rowIndex.value = -1;
    rowID.value = "";
    setForm();
}
const deleteRow = (index) => {
    tableData.value.splice(index, 1)
    resetForm();
}
const deleteRowN = (row) => {
    var index = tableData.value.findIndex((v, index, obj) => v.ID = row.ID);
    tableData.value.splice(index, 1)
    resetForm();
}


</script>
<style scoped>
.agv_main {
    padding: 18px;
    height: 100%;
    box-sizing: border-box;
}


.flex {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding-bottom: 16px;
}

.flex-center {
    display: flex;
    justify-content: center;
}

.bow {
    width: 25px;
    height: 25px;
    border-radius: 50%;
}

.online {
    background-color: green;

}

.offline {
    background-color: red;
}

.hasParam {
    border-color: cadetblue !important;
}

/* .card-header{
    padding: 8px 12px
} */
</style>