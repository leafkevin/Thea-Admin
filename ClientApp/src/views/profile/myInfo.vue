<template>
  <el-row>
    <el-col :span="12">
      <el-form ref="formRef" :model="ruleForm" :rules="rules" label-width="140px">
        <el-form-item label="姓名" prop="name">
          <el-input v-model="ruleForm.name" placeholder="请输入姓名，必填" clearable />
        </el-form-item>
        <el-form-item label="账号" prop="account">
          <el-input v-model="ruleForm.account" placeholder="请输入登录账号，必填" clearable />
        </el-form-item>
        <el-form-item label="手机号码">
          <el-input v-model="ruleForm.mobile" placeholder="请输入会员手机号码，必填" clearable />
        </el-form-item>
        <el-form-item label="邮件地址">
          <el-input v-model="ruleForm.email" placeholder="请输入邮件地址" clearable />
        </el-form-item>
        <el-form-item label="描述">
          <el-input
            v-model="ruleForm.description"
            placeholder="请输入描述信息"
            type="textarea"
            :autosize="{ minRows: 3, maxRows: 5 }"
            clearable />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="submitForm(formRef)"> 保存 </el-button>
          <el-button @click="goBack"> 返回 </el-button>
        </el-form-item>
      </el-form>
    </el-col>
    <el-col :span="12">
      <div class="demo-basic--circle">
        <div class="block">
          <el-avatar :size="100" :src="circleUrl" />
        </div>
      </div>
    </el-col>
  </el-row>
</template>
