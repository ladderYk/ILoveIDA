<template>
    <el-scrollbar style="height: calc(100% - 60px)" aria-orientation="vertical">
        <div style="overflow-x: hidden;">
            <el-row>
                <el-col :span="3" v-for="(data, i) in list">
                    <el-card shadow="hover" body-style="padding: 15px">
                        <div class="flex">
                            <span>
                                {{ i + 1 }}
                            </span>
                            <el-button size="small" :icon="Close" title="删除" @click.prevent="deleteRow1(i)" />
                        </div>
                        <el-input v-model="data.Name" placeholder="值名称" />
                        <el-input v-model="data.Type" placeholder="类型" />
                    </el-card>
                </el-col>
                <el-col :span="3">
                    <el-card style="margin-bottom: 10px;height: 134px;" shadow="hover" body-style="padding: 15px;text-align:center;">
                        <el-button size="large" :icon="Plus" :onclick="onAddItem1">添加</el-button>
                    </el-card>
                </el-col>
            </el-row>
        </div>
    </el-scrollbar>
</template>
<script setup>
import { onMounted, ref, reactive, computed, watch } from "vue";
import { Close, Link, Plus, Refresh } from '@element-plus/icons-vue';
const props = defineProps(['list']);

const list = ref([]);
watch(() => props.list, (newVal, oldVal) => {
    list.value = newVal;
});
const onAddItem1 = () => {
    list.value.push({});
}
const deleteRow1 = (index) => {
    list.value.splice(index, 1);
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

/* .card-header{
    padding: 8px 12px
} */
</style>