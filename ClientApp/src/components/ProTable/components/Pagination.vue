<template>
  <!-- 分页组件 -->
  <el-pagination
    :background="true"
    :current-page="props.pageIndex"
    :page-size="props.pageSize"
    :page-sizes="[15, 25, 50, 100]"
    :total="props.total"
    layout="total, sizes, prev, pager, next, jumper"
    @size-change="handleSizeChange"
    @current-change="handleCurrentChange"></el-pagination>
</template>

<script setup lang="ts" name="Pagination">
  interface IPagination {
    props: IPagingState;
    handleSizeChange: (size: number) => void;
    handleCurrentChange: (currentPage: number) => void;
  }

  defineProps<IPagination>();
</script>
