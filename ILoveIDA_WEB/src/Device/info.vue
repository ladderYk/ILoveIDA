<template>
    <el-tabs>
        <el-tab-pane label="原始数据">
            <el-table :data="initData">
                <el-table-column label="名称" prop="name" />
                <el-table-column label="标签" prop="tag" />
                <el-table-column label="数据" prop="data" />
            </el-table>
        </el-tab-pane>
        <el-tab-pane label="解析数据">
            <JsonFormat v-model="data" />
        </el-tab-pane>
    </el-tabs>
</template>
<script setup>
import { onMounted, ref, reactive } from "vue";
import JsonFormat from '../components/JsonFormat.vue';

const props = defineProps(['form'])
const selProps = {
    label: 'Name',
    value: 'Name',
}
var form = props.form;
const data = {111:"22", 222:true};
const options = ref([]);
const initData = ref([{ name: "数据1", tag: "abc", data: ["0", "1"] }, { name: "数据1", tag: "abc", data: ["0", "1"] }]);
onMounted(() => {
    // get("/DeviceList").then(data => {
    //     tableData.value = data;
    // });
    fetch("/dType.json").then(v => v.json()).then(data => {
        options.value = data;
    });
    // get("/TypeList").then(data => {
    //     options.value = data;
    // });
});
</script>