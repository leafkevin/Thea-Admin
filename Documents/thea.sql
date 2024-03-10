/*
 Navicat Premium Data Transfer

 Source Server         : mariadb
 Source Server Type    : MariaDB
 Source Server Version : 110202 (11.2.2-MariaDB-1:11.2.2+maria~ubu2204)
 Source Host           : localhost:3306
 Source Schema         : salon

 Target Server Type    : MariaDB
 Target Server Version : 110202 (11.2.2-MariaDB-1:11.2.2+maria~ubu2204)
 File Encoding         : 65001

 Date: 10/03/2024 14:52:54
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for mos_balance
-- ----------------------------
DROP TABLE IF EXISTS `mos_balance`;
CREATE TABLE `mos_balance`  (
  `MemberId` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '会员ID',
  `Balance` double(10, 2) NULL DEFAULT NULL COMMENT '余额',
  `ExpiryDate` datetime NULL DEFAULT NULL COMMENT '有效期',
  `ExpectedTimes` int(11) NULL DEFAULT NULL COMMENT '预计使用次数',
  `Status` tinyint(4) NOT NULL COMMENT '状态',
  `CreatedBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '创建人',
  `CreatedAt` datetime NOT NULL DEFAULT current_timestamp() COMMENT '创建日期',
  `UpdatedBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '最后更新人',
  `UpdatedAt` datetime NOT NULL DEFAULT current_timestamp() COMMENT '最后更新日期',
  PRIMARY KEY (`MemberId`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '会员余额表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of mos_balance
-- ----------------------------

-- ----------------------------
-- Table structure for mos_deposit
-- ----------------------------
DROP TABLE IF EXISTS `mos_deposit`;
CREATE TABLE `mos_deposit`  (
  `DepositId` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '充值ID',
  `MemberId` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '会员ID',
  `Amount` double(10, 2) NOT NULL COMMENT '充值金额',
  `Bonus` double(10, 2) NULL DEFAULT NULL COMMENT '赠送金额',
  `BeginBalance` double(10, 2) NULL DEFAULT NULL COMMENT '充值前余额',
  `EndBalance` double(10, 2) NULL DEFAULT NULL COMMENT '充值后余额',
  `Description` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '描述',
  `Status` tinyint(4) NOT NULL COMMENT '状态',
  `CreatedBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '创建人',
  `CreatedAt` datetime NOT NULL DEFAULT current_timestamp() COMMENT '创建日期',
  `UpdatedBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '最后更新人',
  `UpdatedAt` datetime NOT NULL DEFAULT current_timestamp() COMMENT '最后更新日期',
  PRIMARY KEY (`DepositId`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '充值表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of mos_deposit
-- ----------------------------

-- ----------------------------
-- Table structure for mos_member
-- ----------------------------
DROP TABLE IF EXISTS `mos_member`;
CREATE TABLE `mos_member`  (
  `MemberId` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '会员ID',
  `MemberName` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '姓名',
  `Mobile` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '手机号码',
  `Description` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '描述',
  `Gender` tinyint(4) NULL DEFAULT NULL COMMENT '性别',
  `Balance` double(10, 2) NULL DEFAULT NULL COMMENT '余额',
  `Status` tinyint(4) NOT NULL COMMENT '状态',
  `CreatedBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '创建人',
  `CreatedAt` datetime NOT NULL DEFAULT current_timestamp() COMMENT '创建日期',
  `UpdatedBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '最后更新人',
  `UpdatedAt` datetime NOT NULL DEFAULT current_timestamp() COMMENT '最后更新日期',
  PRIMARY KEY (`MemberId`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '会员表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of mos_member
-- ----------------------------
INSERT INTO `mos_member` VALUES ('65dd407f8d0be6d9a48932e1', 'kevin', '18516063052', '1111', 1, 1240.00, 1, '1', '2024-02-24 07:33:25', '1', '2024-03-02 15:51:34');
INSERT INTO `mos_member` VALUES ('65dd407f8d0be6d9a48932e2', 'cindy', '18516063025', '2222', 2, 300.00, 1, '1', '2024-02-24 07:34:16', '1', '2024-02-27 09:48:55');
INSERT INTO `mos_member` VALUES ('65dd407f8d0be6d9a48932f0', 'xiyuan', '18516521234', '234234', 1, 2000.00, 1, '1', '2024-02-27 09:53:03', '1', '2024-03-02 15:54:50');
INSERT INTO `mos_member` VALUES ('65e2dbaea71eb0f22e2fc588', '安刚', '18516063052', '描述信息', 1, 200.00, 1, '1', '2024-03-02 15:56:30', '1', '2024-03-02 15:56:30');

-- ----------------------------
-- Table structure for mos_order
-- ----------------------------
DROP TABLE IF EXISTS `mos_order`;
CREATE TABLE `mos_order`  (
  `OrderId` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '订单ID',
  `MemberId` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '会员ID',
  `StylistId` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '设计师ID',
  `IsAppointed` tinyint(1) NULL DEFAULT 0 COMMENT '是否指定理发师',
  `Description` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '描述',
  `Amount` double(10, 2) NULL DEFAULT NULL COMMENT '消费余额',
  `Status` tinyint(4) NOT NULL COMMENT '状态',
  `CreatedBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '创建人',
  `CreatedAt` datetime NOT NULL DEFAULT current_timestamp() COMMENT '创建日期',
  `UpdatedBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '最后更新人',
  `UpdatedAt` datetime NOT NULL DEFAULT current_timestamp() COMMENT '最后更新日期',
  PRIMARY KEY (`OrderId`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '会员订单表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of mos_order
-- ----------------------------

-- ----------------------------
-- Table structure for mos_stylist
-- ----------------------------
DROP TABLE IF EXISTS `mos_stylist`;
CREATE TABLE `mos_stylist`  (
  `UserId` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '设计师ID',
  `Name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '姓名',
  `Account` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '账号',
  `Mobile` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '手机号码',
  `Email` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '邮件地址',
  `Description` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '描述',
  `AvatarUrl` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '头像地址',
  `Password` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '密码',
  `Salt` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '盐',
  `LockoutEnd` datetime NULL DEFAULT NULL COMMENT '解锁时间',
  `Status` tinyint(4) NOT NULL COMMENT '状态',
  `CreatedBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '创建人',
  `CreatedAt` datetime NOT NULL DEFAULT current_timestamp() COMMENT '创建日期',
  `UpdatedBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '最后更新人',
  `UpdatedAt` datetime NOT NULL DEFAULT current_timestamp() COMMENT '最后更新日期',
  PRIMARY KEY (`UserId`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '设计师表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of mos_stylist
-- ----------------------------

-- ----------------------------
-- Table structure for sys_menu
-- ----------------------------
DROP TABLE IF EXISTS `sys_menu`;
CREATE TABLE `sys_menu`  (
  `MenuId` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '菜单ID',
  `MenuName` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '菜单名称',
  `RouteName` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '路由名称',
  `RouteUrl` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '路由地址',
  `RedirectUrl` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '重定向URL',
  `Description` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '描述',
  `ParentId` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '上级菜单ID',
  `MenuType` tinyint(4) NULL DEFAULT NULL COMMENT '菜单类型',
  `Icon` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '图标',
  `IsStatic` tinyint(1) NULL DEFAULT 0 COMMENT '是否静态路由',
  `Sequence` int(11) NULL DEFAULT NULL COMMENT '序号',
  `Status` tinyint(4) NOT NULL DEFAULT 1 COMMENT '状态',
  `CreatedBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '创建人',
  `CreatedAt` datetime NOT NULL DEFAULT current_timestamp() COMMENT '创建日期',
  `UpdatedBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '最后更新人',
  `UpdatedAt` datetime NOT NULL DEFAULT current_timestamp() COMMENT '最后更新日期',
  PRIMARY KEY (`MenuId`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '菜单表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sys_menu
-- ----------------------------
INSERT INTO `sys_menu` VALUES ('1', '首页', 'home', '/home', '/home/index', NULL, 'AdminRoot', 3, 'HomeFilled', 1, 1, 1, '1', '2024-03-03 01:06:40', '1', '2024-03-03 01:06:40');
INSERT INTO `sys_menu` VALUES ('2', '会员管理', 'memberMgt', '/memberMgt', '/member/index', NULL, 'AdminRoot', 2, 'Avatar', 0, 2, 1, '1', '2024-03-03 01:06:40', '1', '2024-03-03 01:06:40');
INSERT INTO `sys_menu` VALUES ('21', '会员列表', 'memberList', '/member', NULL, NULL, '2', 3, 'UserFilled', 0, 1, 1, '1', '2024-03-03 01:06:40', '1', '2024-03-03 01:06:40');
INSERT INTO `sys_menu` VALUES ('22', '充值管理', 'depositList', '/deposit', NULL, NULL, '2', 3, 'UserFilled', 0, 2, 1, '1', '2024-03-03 01:06:40', '1', '2024-03-03 01:06:40');
INSERT INTO `sys_menu` VALUES ('9', '系统管理', 'systemMgt', '/systemMgt', NULL, NULL, 'AdminRoot', 2, 'Avatar', 0, 9, 1, '1', '2024-03-03 01:06:40', '1', '2024-03-03 01:06:40');
INSERT INTO `sys_menu` VALUES ('91', '用户管理', 'userList', '/user', NULL, NULL, '9', 3, 'UserFilled', 0, 1, 1, '1', '2024-03-03 01:06:40', '1', '2024-03-03 01:06:40');
INSERT INTO `sys_menu` VALUES ('92', '角色管理', 'roleList', '/role', NULL, '管理角色', '9', 3, 'UserFilled', 0, 2, 1, '1', '2024-03-03 01:06:40', '1', '2024-03-03 01:06:40');
INSERT INTO `sys_menu` VALUES ('93', '菜单管理', 'menuList', '/menu', NULL, NULL, '9', 3, 'UserFilled', 0, 3, 1, '1', '2024-03-03 01:06:40', '1', '2024-03-03 01:06:40');
INSERT INTO `sys_menu` VALUES ('94', '授权管理', 'authList', '/auth', NULL, '给用户分配角色，并分配操作菜单', '9', 3, 'UserFilled', 0, 4, 1, '1', '2024-03-03 01:06:40', '1', '2024-03-03 01:06:40');
INSERT INTO `sys_menu` VALUES ('AdminRoot', '管理员角色根菜单', 'AdminRoot', '/', NULL, NULL, NULL, 1, NULL, 0, 1, 1, '1', '2024-03-03 01:06:40', '1', '2024-03-03 01:06:40');

-- ----------------------------
-- Table structure for sys_role
-- ----------------------------
DROP TABLE IF EXISTS `sys_role`;
CREATE TABLE `sys_role`  (
  `RoleId` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '角色ID',
  `RoleName` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '角色名称',
  `Description` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '描述',
  `Status` tinyint(4) NOT NULL COMMENT '状态',
  `CreatedBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '创建人',
  `CreatedAt` datetime NOT NULL DEFAULT current_timestamp() COMMENT '创建日期',
  `UpdatedBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '最后更新人',
  `UpdatedAt` datetime NOT NULL DEFAULT current_timestamp() COMMENT '最后更新日期',
  PRIMARY KEY (`RoleId`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '角色表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sys_role
-- ----------------------------

-- ----------------------------
-- Table structure for sys_role_menu
-- ----------------------------
DROP TABLE IF EXISTS `sys_role_menu`;
CREATE TABLE `sys_role_menu`  (
  `RoleId` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '角色ID',
  `MenuId` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '菜单ID',
  `UpdatedBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '最后更新人',
  `UpdatedAt` datetime NOT NULL DEFAULT current_timestamp() COMMENT '最后更新日期',
  PRIMARY KEY (`RoleId`, `MenuId`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '角色菜单关联表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sys_role_menu
-- ----------------------------
INSERT INTO `sys_role_menu` VALUES ('1', 'AdminRoot', '1', '2024-03-05 13:06:31');

-- ----------------------------
-- Table structure for sys_route
-- ----------------------------
DROP TABLE IF EXISTS `sys_route`;
CREATE TABLE `sys_route`  (
  `RouteId` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '路由ID',
  `RouteName` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '路由名称',
  `RouteTitle` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '路由标题',
  `RouteUrl` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '路由地址',
  `Component` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '组件物理路径',
  `Description` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '描述',
  `MenuId` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '菜单ID',
  `RedirectUrl` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '重定向URL',
  `Icon` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '图标',
  `IsStatic` tinyint(1) NULL DEFAULT 0 COMMENT '是否静态路由',
  `IsHidden` tinyint(1) NULL DEFAULT 0 COMMENT '是否需要隐藏',
  `IsLink` tinyint(1) NULL DEFAULT 0 COMMENT '是否外部连接',
  `IsFull` tinyint(1) NULL DEFAULT 0 COMMENT '是否全屏显示',
  `IsAffix` tinyint(1) NULL DEFAULT 0 COMMENT '是否固定标签页',
  `IsKeepAlive` tinyint(1) NULL DEFAULT 1 COMMENT '是否缓存',
  `Status` tinyint(4) NOT NULL DEFAULT 1 COMMENT '状态',
  `CreatedBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '创建人',
  `CreatedAt` datetime NOT NULL DEFAULT current_timestamp() COMMENT '创建日期',
  `UpdatedBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '最后更新人',
  `UpdatedAt` datetime NOT NULL DEFAULT current_timestamp() COMMENT '最后更新日期',
  PRIMARY KEY (`RouteId`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '页面路由表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sys_route
-- ----------------------------
INSERT INTO `sys_route` VALUES ('11', 'Home', '首页', '/home/index', '/home/index', NULL, '1', NULL, 'UserFilled', 1, 0, 0, 0, 1, 1, 1, '1', '2024-03-03 01:06:40', '1', '2024-03-03 01:06:40');
INSERT INTO `sys_route` VALUES ('12', 'Login', '登录', '/login/index', '/login/index', NULL, NULL, NULL, 'UserFilled', 1, 0, 0, 0, 0, 1, 0, '1', '2024-03-03 01:06:40', '1', '2024-03-03 01:06:40');
INSERT INTO `sys_route` VALUES ('13', 'SwitchRole', '切换角色', '/switchRole/index', '/switchRole/index', NULL, NULL, NULL, 'UserFilled', 1, 0, 0, 0, 0, 1, 0, '1', '2024-03-03 01:06:40', '1', '2024-03-03 01:06:40');
INSERT INTO `sys_route` VALUES ('14', 'Profile', '个人信息', '/profile/index', '/profile/index', NULL, NULL, NULL, 'UserFilled', 1, 0, 0, 0, 0, 1, 0, '1', '2024-03-03 01:06:40', '1', '2024-03-03 01:06:40');
INSERT INTO `sys_route` VALUES ('15', 'ResetPwd', '重置密码', '/resetPwd/index', '/resetPwd/index', NULL, NULL, NULL, 'UserFilled', 1, 0, 0, 0, 0, 1, 0, '1', '2024-03-03 01:06:40', '1', '2024-03-03 01:06:40');
INSERT INTO `sys_route` VALUES ('211', 'MemberList', '会员列表', '/member/index', '/member/index', NULL, '21', NULL, 'UserFilled', 0, 0, 0, 0, 0, 1, 1, '1', '2024-03-03 01:06:40', '1', '2024-03-03 01:06:40');
INSERT INTO `sys_route` VALUES ('212', 'MemberEdit', '会员编辑', '/member/edit', '/member/form', NULL, '21', NULL, 'UserFilled', 0, 1, 0, 0, 0, 1, 1, '1', '2024-03-03 01:06:40', '1', '2024-03-03 01:06:40');
INSERT INTO `sys_route` VALUES ('221', 'DepositList', '充值列表', '/deposit/index', '/deposit/index', NULL, '22', NULL, 'CreditCard', 0, 0, 0, 0, 0, 1, 1, '1', '2024-03-03 01:06:40', '1', '2024-03-03 01:06:40');
INSERT INTO `sys_route` VALUES ('222', 'DepositEdit', '会员充值', '/deposit/edit', '/deposit/form', NULL, '22', NULL, 'CreditCard', 0, 1, 0, 0, 0, 1, 1, '1', '2024-03-03 01:06:40', '1', '2024-03-03 01:06:40');
INSERT INTO `sys_route` VALUES ('911', 'UserList', '用户列表', '/user/index', '/user/index', NULL, '91', NULL, 'UserFilled', 0, 0, 0, 0, 0, 1, 1, '1', '2024-03-03 01:06:40', '1', '2024-03-03 01:06:40');
INSERT INTO `sys_route` VALUES ('912', 'UserEdit', '用户编辑', '/user/edit', '/user/form', NULL, '91', NULL, 'UserFilled', 0, 1, 0, 0, 0, 1, 1, '1', '2024-03-03 01:06:40', '1', '2024-03-03 01:06:40');
INSERT INTO `sys_route` VALUES ('921', 'RoleList', '角色列表', '/role/index', '/role/index', NULL, '92', NULL, 'UserFilled', 0, 0, 0, 0, 0, 1, 1, '1', '2024-03-03 01:06:40', '1', '2024-03-03 01:06:40');
INSERT INTO `sys_route` VALUES ('922', 'RoleEdit', '角色编辑', '/role/edit', '/role/edit', NULL, '92', NULL, 'UserFilled', 0, 1, 0, 0, 0, 1, 1, '1', '2024-03-03 01:06:40', '1', '2024-03-03 01:06:40');
INSERT INTO `sys_route` VALUES ('931', 'MenuList', '菜单列表', '/menu/index', '/menu/index', NULL, '93', NULL, 'UserFilled', 0, 0, 0, 0, 0, 1, 1, '1', '2024-03-03 01:06:40', '1', '2024-03-03 01:06:40');
INSERT INTO `sys_route` VALUES ('932', 'MenurEdit', '菜单编辑', '/menu/edit', '/menu/form', NULL, '93', NULL, 'UserFilled', 0, 1, 0, 0, 0, 1, 1, '1', '2024-03-03 01:06:40', '1', '2024-03-03 01:06:40');
INSERT INTO `sys_route` VALUES ('941', 'AuthList', '授权列表', '/auth/index', '/auth/index', NULL, '94', NULL, 'UserFilled', 0, 0, 0, 0, 0, 1, 1, '1', '2024-03-03 01:06:40', '1', '2024-03-03 01:06:40');
INSERT INTO `sys_route` VALUES ('942', 'AuthEdit', '用户授权', '/auth/form', '/auth/form', NULL, '94', NULL, 'UserFilled', 0, 1, 0, 0, 0, 1, 1, '1', '2024-03-03 01:06:40', '1', '2024-03-03 01:06:40');

-- ----------------------------
-- Table structure for sys_user
-- ----------------------------
DROP TABLE IF EXISTS `sys_user`;
CREATE TABLE `sys_user`  (
  `UserId` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '用户ID',
  `Name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '姓名',
  `Account` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '账号',
  `Mobile` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '手机号码',
  `Email` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '邮件地址',
  `Description` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '描述',
  `AvatarUrl` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '头像地址',
  `Password` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '密码',
  `Salt` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '盐',
  `LockoutEnd` datetime NULL DEFAULT NULL COMMENT '解锁时间',
  `Status` tinyint(4) NOT NULL COMMENT '状态',
  `CreatedBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '创建人',
  `CreatedAt` datetime NOT NULL DEFAULT current_timestamp() COMMENT '创建日期',
  `UpdatedBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '最后更新人',
  `UpdatedAt` datetime NOT NULL DEFAULT current_timestamp() COMMENT '最后更新日期',
  PRIMARY KEY (`UserId`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '用户表,所有登陆系统的用户信息' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sys_user
-- ----------------------------
INSERT INTO `sys_user` VALUES ('1', 'kevin', 'leafkevin', '18516063052', NULL, NULL, NULL, 'AAAAAQAAJxAAAAAQQscVYn2S2q0JXirAF3EezrYMK8nN5qFoneujteplI7qS519HBApIwwa064LiCBrf', 'QscVYn2S2q0JXirAF3Eezg==', NULL, 1, '1', '2024-02-24 00:24:36', '1', '2024-02-24 00:24:36');

-- ----------------------------
-- Table structure for sys_user_role
-- ----------------------------
DROP TABLE IF EXISTS `sys_user_role`;
CREATE TABLE `sys_user_role`  (
  `UserId` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '用户ID',
  `RoleId` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '角色ID',
  `UpdatedBy` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '最后更新人',
  `UpdatedAt` datetime NOT NULL DEFAULT current_timestamp() COMMENT '最后更新日期',
  PRIMARY KEY (`UserId`, `RoleId`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '用户角色关联表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sys_user_role
-- ----------------------------
INSERT INTO `sys_user_role` VALUES ('1', '1', '1', '2024-03-05 13:06:18');

SET FOREIGN_KEY_CHECKS = 1;
