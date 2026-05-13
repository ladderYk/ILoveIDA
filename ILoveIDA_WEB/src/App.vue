<script setup>
import { ref, computed, onMounted } from "vue";
import DeviceType from "./DeviceType/index.vue";
import Device from "./Device/index.vue";
import ProtsIndex from "./Prots/index.vue";
import {
  Fold, Expand
} from '@element-plus/icons-vue'
// import Home from "./pages/Index.vue";
// import About from './About.vue'
// import NotFound from './NotFound.vue'
//let sokect = null;
const textarea = ref([]);
const isCollapse = ref(false)

const routes = {
  "/": ProtsIndex,
  "/Device": DeviceType,
};
const currentPath = ref(window.location.hash || "#/");
window.addEventListener("hashchange", () => {
  currentPath.value = window.location.hash;
});
const currentView = computed(() => {
  return routes[currentPath.value.slice(1) || "/"] || NotFound;
});
const handleSelect = (key, keyPath) => {
  window.location.hash = key;
}
onMounted(() => {
  // sokect = new WebSocket(ws);
  // sokect.onopen = () => {
  //   sokect.send(JSON.stringify({ topic: 'data', action: 'subscribe' }));
  // };
  // sokect.onerror = () => {
  //   // setTimeout(() => {
  //   //   document.location.reload();
  //   // }, 10000);
  // }
  // sokect.onmessage = (message) => {
  //   textarea.value.unshift(message.data);
  // };
});

</script>

<template>
  <!-- <el-header class="flex">
      
    </el-header> -->
  <el-container>

    <el-header> <el-menu class="menu" :default-active="currentPath" style="width:100%" @select="handleSelect"
        mode="horizontal" active-text-color="#1890ff" background-color="#001529" text-color="#fff"
        :collapse="isCollapse">
        <div class="logo">
          <!-- {{isCollapse? "NP":"NetProts"}} -->
          ILoveIDA
        </div>
        <el-menu-item index="#/">
          <el-icon>
            <Operation />
          </el-icon>
          <span>设备协议</span>
        </el-menu-item>
        <el-menu-item index="#/Device">
          <el-icon>
            <Monitor />
          </el-icon>
          <span>设备列表</span>
        </el-menu-item>
      </el-menu></el-header>

    <el-main>
      <!-- <el-col :span="18"> -->
      <component :is="currentView" />
      <!-- </el-col> -->
      <!-- <el-row style="height: 100%;">
         
          <el-col :span="6" style="height: 100%;">
            <el-card header="消息列表" style="height: 100%;box-sizing: border-box;">
              <el-scrollbar>
                <div v-for="v in textarea">
                  <span style="font-size: 12px;">{{ v }}</span>
                </div>
              </el-scrollbar>
            </el-card>
          </el-col>
        </el-row> -->
    </el-main>
  </el-container>
</template>
<style scoped>
main {
  height: calc(100vh - 60px);
  /* height: calc(100vh - 60px); */
}

.el-main {
  padding: 0;
}

/* .agv_main {
    height: calc(100% - 20px);
    background-color: #fff;
  } */

header {
  background-color: #001529;
  color: #fff;
  padding: 0;
}

.flex {
  display: flex;
}

.logo {
  line-height: 60px;
  font-size: 18px;
  text-align: center;
  color: #fff;
  width: 150px;
}

.logo.cos {
  width: auto;
  font-size: 12px;
}

.coll-icon {
  justify-self: end;
  display: flex;
  margin-right: 10px;
  font-size: 18px;
}

.menu:not(.el-menu--collapse) {
  width: 200px;
}

.sf-aside {
  background-color: #001529;
  flex-basis: content;
}
</style>