/**
* Author: ZongAn
* Date: 2023/4/21
* Description:
*/

<template>
  <div class="page">
    <div class="title">登录页面</div>
    <div class="box">
      <button @click="login">登录</button>
    </div>
  </div>
</template>

<script>
import oidcClient from "@/utils/oidc.js";
export default {
  name: '',
  data() {
    return {};
  },
  methods:{
    login() {
      oidcClient.signinRedirect()
    }
  }
};
</script>

<style scoped lang="scss">

</style>