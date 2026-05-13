<template>
    <div class="agv_main">
        <div class="flex">
            <span style="font-size: 18px;">
                设备列表
            </span>
            <el-button @click="addType" type="primary" :icon="Plus">添加设备</el-button>
        </div>
        <div class="agv_list">
            <el-scrollbar v-if="tableData.length > 0" height="300px" >
                <div style="overflow: hidden;">
                <el-row :gutter="15">
                    <el-col :span="6" v-for="data in tableData" :key="data.ID">
                        <el-card shadow="never" style="margin-bottom: 10px;">
                            <el-row>
                                <el-col :span="20">
                                    <div class="flex">
                                        {{ data.Name }}
                                        <el-button-group>
                                            <el-button size="small" :icon="Close" @click="deleteRowN(data)" />
                                            <el-button :type="rowID == data.ID ? 'primary' : ''" size="small"
                                                :icon="Edit" @click="editN(data)" />
                                        </el-button-group>
                                    </div>
                                    <el-text type="info">{{ data.Model }}</el-text>
                                </el-col>
                                <el-col :span="4">
                                    <div class="flex-center">
                                        <div class="bow" :class="data.Online ? 'online' : 'offline'"></div>
                                    </div>
                                </el-col>
                            </el-row>
                        </el-card>
                    </el-col>
                </el-row>
            </div>
            </el-scrollbar>


            <!-- <el-table :data="tableData" style="width: 100%;" border highlight-current-row>
                            <el-table-column type="index" width="50" />
                            <el-table-column prop="Name" label="名称" />
                            <el-table-column prop="Model" label="类型" />
                            <el-table-column prop="IP" label="IP" />
                            <el-table-column prop="Port" label="端口号" />
                            <el-table-column prop="Timeout" label="连接超时时间" />
                            <el-table-column prop="Cycle" label="循环周期" />
                            <el-table-column fixed="right" label="操作">
                                <template #default="scope">
                                    <el-button-group>
                                        <el-button size="small" :icon="Close"
                                            @click.prevent="deleteRow(scope.$index)" />
                                        <el-button :type="rowIndex == scope.$index ? 'primary' : ''" size="small"
                                            :icon="Edit" @click.prevent="edit(scope.$index, scope.row)" />
                                    </el-button-group>
                                </template>
</el-table-column>
</el-table> -->
    <el-divider />
            <InfoPage :form="form" />
            <!-- <span>{{ rowIndex == -1 && rowID == "" ? "添加" : "编辑" }}</span>
                        <el-divider border-style="dashed" />
                        <el-form :model="form" :inline="true" label-position="top" ref="formRef" label-width="auto">
                            <el-form-item prop="name" label="名称" :rules="[
                                { required: true, message: '名称不能为空' }
                            ]">
                                <el-input v-model="form.name" autocomplete="off" />
                            </el-form-item>
                            <el-form-item prop="model" label="类型" :rules="[
                                { required: true, message: '类型不能为空' }
                            ]">
                                <el-select-v2  style="width: 200px" :props="props" :options="options" v-model="form.model" value-key="Name" />
                            </el-form-item>
                            <el-form-item prop="ip" label="IP" :rules="[
                                { required: true, message: 'IP不能为空' }
                            ]">
                                <el-input v-model="form.ip" autocomplete="off" />
                            </el-form-item>
                            <el-form-item prop="port" label="端口号" :rules="[
                                { required: true, message: '端口号不能为空' }
                            ]">
                                <el-input v-model="form.port" autocomplete="off" />
                            </el-form-item>
                            <el-form-item prop="timeout" label="连接超时时间" :rules="[
                                { required: true, message: '连接超时时间不能为空' }
                            ]">
                                <el-input v-model="form.timeout" autocomplete="off" />
                            </el-form-item>
                            <el-form-item prop="cycle" label="循环周期" :rules="[
                                { required: true, message: '循环周期不能为空' }
                            ]">
                                <el-input v-model="form.cycle" autocomplete="off" />
                            </el-form-item>
                        </el-form>
                        <div>
                            <el-button type="primary" @click="submitForm(formRef)">
                                提交
                            </el-button>
                            <el-button @click="resetForm()">重置</el-button>
                        </div> -->
        </div>
    </div>
</template>
<script setup>
// TODO 两个接口
// 编辑/修改，禁用/恢复
import { onMounted, ref, reactive } from "vue";
import { Close, Edit, Plus } from '@element-plus/icons-vue'
import { post, get } from "../utils/request";
import InfoPage from "./info.vue";

let sokect = null;
const showEdit = ref(false)

const formRef = ref();
const tableData = ref([]);
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
const props = {
    label: 'Name',
    value: 'Name',
}
const options = ref([]);
onMounted(() => {
    // get("/DeviceList").then(data => {
    //     tableData.value = data;
    // });
    fetch("/AGV1.json").then(v => v.json()).then(data => {
        tableData.value = data;
    });
    fetch("/dType.json").then(v => v.json()).then(data => {
        options.value = data;
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

/* .card-header{
    padding: 8px 12px
} */
</style>