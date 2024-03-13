<template>
  <div class="card content-box">
    <el-form ref="formRef" :model="ruleForm" :rules="rules" label-width="140px">
      <el-form-item label="会员名称" prop="memberName">
        <el-input v-model="ruleForm.memberName" placeholder="请输入会员名称，必填" clearable />
        <el-input v-model="memberId" type="text" />
      </el-form-item>
      <el-form-item label="会员手机号" prop="mobile">
        <el-input v-model="ruleForm.mobile" placeholder="请输入会员手机号码，必填" clearable />
      </el-form-item>
      <el-form-item label="性别">
        <el-radio-group v-model="ruleForm.gender">
          <el-radio-button label="未知" :value="0" />
          <el-radio-button label="男性" :value="1" />
          <el-radio-button label="女性" :value="2" />
        </el-radio-group>
        <el-input v-model="ruleForm.gender" />
      </el-form-item>
      <el-form-item label="余额" prop="balance">
        <template #prepend>¥</template>
        <el-input v-model="ruleForm.balance" placeholder="请输入充值余额，必填，并且>0" />
      </el-form-item>
      <el-form-item label="描述">
        <el-input v-model="ruleForm.description" placeholder="请输入描述信息" type="textarea" clearable />
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="submitForm(formRef)"> 保存 </el-button>
        <el-button @click="resetForm(formRef)"> 重置 </el-button>
      </el-form-item>
    </el-form>
  </div>
</template>

<script setup lang="ts">
  import { reactive, ref } from "vue";
  import { useRoute, useRouter } from "vue-router";
  import { checkPhoneNumber } from "@/utils/eleValidate";
  import type { FormInstance, FormRules } from "element-plus";
  import { ElNotification } from "element-plus";
  import { createMember } from "@/api/member";
  import { useTabPageStore } from "@/stores/tabPages";

  defineOptions({
    name: "MemberEdit"
  });

  const formRef = ref<FormInstance>();
  const currentRoute = useRoute();
  const memberId = ref(currentRoute.params.id as string);
  const ruleForm = reactive({
    memberId: (currentRoute.params.id as string) ?? "",
    memberName: "",
    mobile: "",
    gender: 0,
    balance: 0.0,
    description: ""
  });

  const rules = reactive<FormRules>({
    memberName: [{ required: true, message: "请输入会员名称", trigger: "blur" }],
    mobile: [{ required: true, validator: checkPhoneNumber, trigger: "blur" }],
    balance: [{ required: true, message: "请输入充值余额，必填，并且>0", trigger: "blur" }]
  });

  const submitForm = async (formEl: FormInstance | undefined) => {
    if (!formEl) return;
    await formEl.validate(async (valid, fields) => {
      if (valid) {
        const response = await createMember(ruleForm);
        if (!response.isSuccess) {
          ElNotification({
            title: "操作失败",
            message: `${response.message}，code:${response.code}`,
            type: "error"
          });
        }
        const router = useRouter();
        const tabPageStore = useTabPageStore();
        tabPageStore.removeTabPage(router.currentRoute.value.fullPath);
        router.push({ name: "MemberList", replace: true });
        ElNotification({
          title: "操作成功",
          message: `操作成功，返回会员列表`,
          type: "success",
          duration: 3000
        });
      } else {
        console.log("error submit!", fields);
      }
    });
  };

  const resetForm = (formEl: FormInstance | undefined) => {
    if (!formEl) return;
    formEl.resetFields();
  };
</script>
<style scoped lang="scss">
  @import "./index.scss";
</style>
