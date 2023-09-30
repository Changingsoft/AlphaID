/**
 * Author: ZongAn
 * Date: 2023/4/21
 * Description:
 */
import { createRouter, createWebHistory } from 'vue-router'


export const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      name: 'Index',
      component: () => import('@/views/index.vue'),
    },
    {
      path: '/callback',
      name: 'CallBack',
      component: () => import('@/views/callback.vue'),
    },
  ]
})
