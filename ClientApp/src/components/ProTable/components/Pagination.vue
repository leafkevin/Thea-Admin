<template>
  <!-- 分页组件 -->
  <el-pagination
    :background="true"
    :current-page="props.pageNumber"
    :page-size="props.pageSize"
    :page-sizes="[10, 25, 50, 100]"
    :total="props.total"
    layout="total, sizes, prev, pager, next, jumper"
    @size-change="handleSizeChange"
    @current-change="handleCurrentChange"></el-pagination>
</template>

<script setup lang="ts" name="Pagination">
  interface IPagingProps {
    pageNumber: number;
    pageSize: number;
    total: number;
  }

  interface IPagination {
    props: IPagingProps;
    handleSizeChange: (size: number) => void;
    handleCurrentChange: (currentPage: number) => void;
  }

  defineProps<IPagination>();
</script>
