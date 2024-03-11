<template>
  <div class="table-box">
    <ProTable
      ref="tableRef"
      title="会员列表"
      row-key="memberId"
      :indent="20"
      :columns="columns"
      :request-api="getTableList"
      highlight-current-row>
      <!-- 表格 header 按钮 -->
      <template #tableHeader>
        <el-button type="primary" :icon="CirclePlus">新增会员</el-button>
        <!-- <el-button type="primary" :icon="CirclePlus" @click="openDrawer('新增')">新增用户</el-button>
        <el-button type="primary" :icon="Upload" plain @click="batchAdd">批量添加用户</el-button>
        <el-button type="primary" :icon="Download" plain @click="downloadFile">导出用户数据</el-button>
        <el-button type="danger" :icon="Delete" plain :disabled="!scope.isSelected" @click="batchDelete(scope.selectedListIds)">
          批量删除用户
        </el-button> -->
      </template>
      <!-- 菜单图标 -->
      <template #icon="scope">
        <el-icon :size="18">
          <component :is="scope.row.meta.icon"></component>
        </el-icon>
      </template>
      <!-- 菜单操作 -->
      <template #operation>
        <el-button type="primary" link :icon="EditPen"> 编辑 </el-button>
        <el-button type="primary" link :icon="Delete"> 删除 </el-button>
      </template>
    </ProTable>
  </div>
</template>

<script setup lang="ts">
  import { ref } from "vue";
  import { ColumnProps, ProTableInstance } from "@/components/ProTable/interface";
  import { Delete, EditPen, CirclePlus, Upload, Download } from "@element-plus/icons-vue";
  import ProTable from "@/components/ProTable/index.vue";

  defineOptions({
    name: "MemberList"
  });

  const loading = ref(false);
  const tableRef = ref<ProTableInstance>();

  // 表格配置项
  const columns: ColumnProps[] = [
    { type: "selection", fixed: "left", width: 70 },
    { prop: "memberId", label: "会员ID", align: "left" },
    { prop: "memberName", label: "会员姓名", align: "center", search: { el: "input" } },
    { prop: "mobile", label: "手机号码", align: "center", search: { el: "input" } },
    {
      prop: "gender",
      label: "性别",
      render: scope => {
        switch (scope.row.gender) {
          case 0:
            return <el-tag type="info">未知</el-tag>;
          case 1:
            return <el-tag type="primary">男性</el-tag>;
          case 2:
            return <el-tag type="danger">女性</el-tag>;
        }
      }
    },
    { prop: "balance", label: "余额", align: "right", width: 300, search: { el: "input" } },
    { prop: "component", label: "组件路径", width: 300 },
    { prop: "operation", label: "操作", width: 250, fixed: "right" }
  ];
  async function getTableList() {
    console.log("ok");
  }
</script>
