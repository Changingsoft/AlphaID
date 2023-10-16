# Deployment Guide

## Requirements

### Your organization IT infrastructures

* Email system

An email system or email service will be used for email address validation, reset password, etc. Alpha ID uses SMTP protocol for mail sending.

* Mobile phone short message service

Short message service will be used for phone number verification, reset password, receive notice, etc.

## Multi instances & Load balance

Alpha ID 支持多实例部署。

运行实例所需资源有差异时，应在负载均衡器上调整分发比例，以避免遇到性能瓶颈。

HTTP代理或负载均衡器可以使用根目录下的 /Heart，检测实例是否处于工作状态。

