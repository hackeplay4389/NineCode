-- 创建数据库结构SQL脚本
-- Version: 1.1
-- Time: 2018年1月17日
-- Run: SQL Server2012
-- To：SQL 2008R2+
-- By: Jiuone


--创建数据库（便于测试）
--检测重名
use master
go
if exists(select * from sys.databases where name='NineCode')
	drop database NineCode
go
--创建数据库
create database NineCode
go

--切换数据库
use NineCode
go



--创建用户表
create table Users
(
--用户编号
UNum int primary key identity(10000,1),
--用户名称
UName nvarchar(20) unique not null,
--用户密码//加密后
UPass nvarchar(300) not null,
--安全邮箱
UMail nvarchar(50) null,
--活跃时间
ULogin datetime default(getdate()),
--账户状态
UState nvarchar(5) default('true') check(UState='true' or UState='false'),
)
go

--创建分类表
create table Category
(
--分类编号
CID int primary key identity(100,1),
--分类名称
CName nvarchar(30) not null
)
go

--创建标签表
create table Tag
(
--技术编号
TNum int primary key identity(100,1),
--技术名称
TName nvarchar(50) not null
)
go

--创建文章表
create table Article
(
--文章编号
AID int primary key identity(10000,1),
--文章标题
ATitle nvarchar(100) not null,
--文章内容
AText text not null,
--发布时间
ATime datetime default(getDate()),
--操作用户
UNum int references Users(UNum),
--所属分类
CID int references Category(CID)
)
go

--创建关系表
create table Relation
(
--关系编号
RID int primary key identity(10000,1),
--文章编号
AID int references Article(AID),
--技术编号
TNum int references Tag(TNum)
)
go

--创建文件表
create table Media
(
--文件编号
MID int primary key identity(10000,1),
--文件名称
MName nvarchar(100) not null,
--文件地址
MUrl nvarchar(200) unique not null,
--文件类型
MType nvarchar(10) not null,
--上传时间
MTime datetime default(getDate()),
--所属文章
AID int null,
--操作用户
UNum int references Users(UNum)
)
go


--创建网站信息表
create table SiteInfo
(
--ID标识
IID int primary key identity(1,1),
--网站标题
ITitle nvarchar(100) not null ,
--网站副标题
ISmall nvarchar(100) not null ,
--网站SEO信息
ISEO nvarchar(200),
--网站ICP备案信息
IICP nvarchar(100),
--网站公安部备案信息
IBei nvarchar(100),
--网站统计信息
ICount text,
--网站是否关闭
IIsOff nvarchar(5) default('false') check(IIsOff='true' or IIsOff='false'),
--网站关闭原因
IWhyOff nvarchar(500)
)
go



--创建网站配置表
create table SiteConfig
(
--ID标识
CfgID int primary key identity(1,1),
--SMTP主机地址
CfgServer nvarchar(100),
--SMTP地址端口
CfgPort int,
--SMTP账号
CfgUser nvarchar(100),
--SMTP密码
CfgPass nvarchar(100),
--SMTP发件人地址
CfgFrom nvarchar(100)
)
go
 