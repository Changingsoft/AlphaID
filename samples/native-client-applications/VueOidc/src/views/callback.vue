/**
* Author: ZongAn
* Date: 2023/4/21
* Description:
*/

<template>
  <div class="page">
    <div class="title">回调页面</div>
    <div class="box">
      <pre>
         {{ userInfo }}
      </pre>
    </div>
    <div v-if="userInfo" style="display: flex;justify-content: center;">
      <button @click="signOut">退出登录</button>
    </div>
  </div>
</template>

<script>
import oidcClient from '@/utils/oidc'
export default {
  name: 'CallBack',
  data() {
    return {
      userInfo: ''
    };
  },
  async mounted() {
    // 用于刷新回调页面时获取用户信息，首次登录时回调该页面不会有user信息
    let user = await oidcClient.getUser()
    if(!user) user = await oidcClient.signinCallback();
    this.userInfo = JSON.stringify(user, null, 2)
  },
  methods:{
    signOut() {
      oidcClient.signoutRedirect()
    }
  }
};
</script>

<style scoped>
pre{
  width: 100%;
  text-indent: -6em;
  white-space: break-spaces ;
  word-break: break-all;
}
</style>