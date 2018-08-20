
-- 创建数据表存储过程SQL脚本
-- Version: 1.1
-- Time: 2018年1月17日
-- Run: SQL Server2012
-- To：SQL 2008R2+
-- By: Jiuone

  
--切换数据库
use NineCode
go


--账户相关信息统计
--输入:UNum
--输出:统计信息详情
create proc GetCount
(
@num int
)
as
select
(select COUNT(AID) from Article where UNum=@num)as ACount,
(select COUNT(MID) from Media where UNum=@num) as MCount,
(select ULogin from Users where UNum=@num) as ULogin  
go

--执行测试
--exec GetCount 10000
go


---------------------------------------------
--帐号使用相关操作
---------------------------------------------

--账户登录验证
--输入:用户名密码
--输出:正确:用户ID 错误:false
create proc CheckLogin
(
@name nvarchar(20),
@pass nvarchar(300)
)
as
declare @res nvarchar(50)
select @res=UState from Users where UName=@name
if(@res='false')
begin
	select 'State'
	return
end
select 
	@res=UPass COLLATE Chinese_PRC_CS_AI
from 
	Users 
where 
	UName COLLATE Chinese_PRC_CS_AI =@name 
if(@res COLLATE Chinese_PRC_CS_AI=@pass COLLATE Chinese_PRC_CS_AI)
begin
	select UNum from Users where UName=@name
	update Users set ULogin=getdate() where UName=@name
end
else
	select 'false'
go
--执行测试
--exec CheckLogin 'user','123456'
go


create proc GetMailByName
(
@name nvarchar(20)
)
as
declare @res int
select @res=COUNT(UMail) from Users where UName=@name
if(@res=0)
	select 'false'
else
begin
	select UMail from Users where UName=@name
end
go

--执行测试
--exec GetMailByName 'admin'
go



---------------------------------------------
--会员管理相关操作
---------------------------------------------
--获取网站所有普通管理员信息
--输入：无
--输出：网站管理员信息列表
create proc GetUserList
as
select
	UNum,
	UName,
	UMail,
	ULogin,
	UState,
	(select 
		count(AID) 
	 from 
		Article a 
	 where 
		a.UNum=u.UNum 
	)as ACount
from 
	Users u 
where 
	UNum!=10000
go

--执行测试
--exec GetUserList
go


--操作普通管理员账户
--输入：S/D/R操作指令、用户UNum、加密后默认新密码
--输出：true、false
create proc MgrAdmin
(
@type nvarchar(2),
@num int,
@newpass nvarchar(300)=''
)
as
begin transaction
if(@type='S')
begin
	update Users set UState='true' where UNum=@num
end
else if(@type='D')
begin
	update Users set UState='false' where UNum=@num
end
else if(@type='R')
begin
	update Users set UState='true',UMail=null,UPass=@newpass where UNum=@num
end
else
begin
	select 'false'
	return
end
if(@@ERROR=0)
begin
	commit transaction
	select 'true'
end
else
begin
	rollback transaction
	select 'false'
end
go

--执行测试
--exec MgrAdmin 'S',10001
go

--添加网站管理员
--输入：
--输出：
create proc AddAdminUser
(
@name nvarchar(20),
@pass nvarchar(300),
@mail nvarchar(50)=null
)
as
begin transaction
declare @res nvarchar(20)='N'
select @res=UName from Users where UName=@name
if(@res='N')
begin
insert into Users values(@name,@pass,@mail,default,default)
if(@@ERROR=0)
begin
	commit transaction 
	select 'true'
end
else
begin
	rollback transaction
	select 'false'
end
end
else
select 'UName'
go

--执行测试
--exec AddAdminUser 'test2','123456',''
go


---------------------------------------------
--个人资料相关操作
---------------------------------------------
--获取指定用户信息
--输入：无
--输出：网站管理员信息列表
create proc GetUserInfoById
(
@unum int
)
as
select
	UNum,
	UName,
	UMail,
	ULogin,
	UState,
	(select 
		count(AID) 
	 from 
		Article a 
	 where 
		a.UNum=u.UNum 
	)as ACount
from 
	Users u 
where 
	UNum=@unum
go

--执行测试
--exec GetUserInfoById 10000
go




---------------------------------------------
--站点信息相关操作
---------------------------------------------

--返回站点基本信息
--输入：站点信息ID 默认为1
--输出：站点基本信息
create proc LoadSiteInfo
(
@id int=1 --默认值
)
as
select * from SiteInfo where IID=@id
go

--执行测试
--exec LoadSiteInfo
go



---------------------------------------------
--文章管理相关操作
---------------------------------------------

--新建文章加载
--输入 操作人ID UNum
--输出 新文章ID
create proc NewArticel
(
@id nvarchar(20)
)
as
begin transaction
insert into Article values('Null_Title','NineCode',default,@id,100)
if(@@ERROR=0)
begin
	commit transaction
	select top 1 @id=AID from Article order by ATime desc
	select @id
end
else
begin
	rollback transaction
	select 'false'
end
go

--执行测试
--exec NewArticel 10000
go


--清除空文章冗余
--输入: 无
--输出: true/false 字符串
create proc ClearNullArticle
as
begin transaction
delete from Article where ATitle='Null_Title'
if(@@ERROR=0)
begin
	commit transaction 
	select 'true'
end
else
begin
	rollback transaction
	select 'false'
end
go

--执行测试
--exec ClearNullArticle
go


--更新文章
--输出：文章编号,文章标题,文章内容,操作用户,所属分类
--输出：true/false字符串
create proc UpArticle
(
--文章编号
@AID int ,
--文章标题
@ATitle nvarchar(100) ,
--文章内容
@AText text ,
--所属分类
@CID int,
--操作用户
@UNum int
)
as
begin transaction 
update 
	Article 
set 
	ATitle=@ATitle,
	AText=@AText,
	ATime=GETDATE(),
	UNum=@UNum,
	CID=@CID 
where 
	AID=@AID
if(@@ERROR=0)
begin
	commit transaction 
	select 'true'
end
else
begin
	rollback transaction
	select 'false'
end
go

--执行测试
--exec UpArticle 10000,'更新测试','更新的内容',101,10000
go


--删除文章
--输入：文章ID
--输出：true/false字符串
create proc DelArticle
(
@id int
)
as
begin transaction
--更新附件信息
update Media set AID=0 where AID=@id
--删除标签关系
delete from Relation where AID=@id
--删除文章
delete from Article where AID=@id
if(@@ERROR=0)
begin
	commit transaction
	select 'true'
end
else
begin
	rollback transaction
	select 'false'
end
go

--执行测试
--exec DelArticle 10009
go


--获取文章列表
--输入：分类ID 默认 查询所有
--输出：指定分类的文章列表
create proc GetArticleByCID
(
@cid nvarchar(50)=''
)
as
if(@cid!='')
	set @cid='and c.CID='+@cid
declare @sql nvarchar(max)
set @sql=
'select 
	a.AID,
	a.ATitle,
	u.UName,
	c.CName,
	CONVERT(nvarchar(111),ATime,23)as ATime
from 
	Article a, 
	Users u,
	Category c 
where 
	a.UNum=u.UNum 
	and a.CID=c.CID '
set @sql=@sql+@cid
exec (@sql)
go

--执行测试
--exec GetArticleByCID ''
--exec GetArticleByCID '101'
go



---------------------------------------------
--附件管理相关操作
---------------------------------------------

--添加附件
--输入：附件名，附件地址，附件类型，操作人ID，文章ID(默认0)
--输出：true/false 字符串
create proc AddFile
(
@name nvarchar(100),
@url nvarchar(200),
@type nvarchar(10),
@unum int,
@aid int=0
)
as
begin transaction
declare @res int
insert into Media values(@name,@url,@type,default,@aid,@unum) 
if(@@ERROR=0)
begin
	commit transaction
	select 'true'
end
else
begin
	rollback transaction
	select 'false'
end
go

--执行测试
--exec AddFile 'test4.png','/UpLoad/2018/2/20180202155326.jpg','image',10000,10005
go


--删除附件
--输入：附件ID
--输出：true/false 字符串
create proc DelMediaFile
(
@mid int
)
as
begin transaction
delete from Media where MID=@mid
if(@@ERROR=0)
begin
	commit transaction
	select 'true'
end
else
begin
	rollback transaction
	select 'false'
end
go

--执行测试
--exec DelMediaFile 10000
go

--获取附件URL
--输入：附件ID
--输出：附件URL
create proc GetUrlById
(
@mid int
)
as
select 
	MUrl 
from 
	Media 
where 
	MID=@mid
go

--执行测试
--exec GetUrlById 10000


--获取媒体库列表
--输入：文件类型/部分文件标题、查询类型T/K
--输出：媒体库详情列表（最新时间靠前，排除有文章关联的）
create proc GetMediaList
(
@by nvarchar(2)='T',
@key nvarchar(200)='0'
)
as
if(@by='T'and @key='0')
begin
select
	m.MID,
	m.MName,
	m.MTime,
	m.MType,
	m.MUrl,
	u.UName
from 
	Media m,
	Users u 
where 
	m.UNum=u.UNum 
	and AID=0  
order by
	MTime desc
end
else if(@by='T'and @key!='0')
begin
select
	m.MID,
	m.MName,
	m.MTime,
	m.MType,
	m.MUrl,
	u.UName
from 
	Media m,
	Users u 
where 
	m.UNum=u.UNum 
	and AID=0  
	and MType=@key
order by
	MTime desc
end
else if(@by='K'and @key!='0')
begin
select
	m.MID,
	m.MName,
	m.MTime,
	m.MType,
	m.MUrl,
	u.UName
from 
	Media m,
	Users u 
where 
	m.UNum=u.UNum 
	and AID=0 
	and MName like '%'+@key+'%'
order by
	MTime desc
end
go

--执行测试
--exec GetMediaList 
--exec GetMediaList 'T','ZIP'
--exec GetMediaList 'K','2'
go

--获取所有文件类型列表
--输入：无
--输出：所有文件类型列表
create proc GetMediaType
as
select 
	MType
from 
	Media
group by 
	MType 
order by 
	MType asc 
go

--执行测试
--exec GetMediaType


--获取文章图片
--输入：文章ID/部分文章标题、查询类型N/T
--输出：文章详情列表（最新时间靠前）
create proc GetArticileImg
(
@by nvarchar(2)='D',
@key nvarchar(200)='0'
)
as
if(@by='D')
	select
		m.MID,
		m.MName,
		m.MUrl
	from
		Media  m,
		Article a
	where 
		m.AID=a.AID
		and m.AID!=0
	order by m.MTime desc
else if(@by='N')
	select
		m.MID,
		m.MName,
		m.MUrl
	from
		Media  m,
		Article a
	where 
		m.AID=a.AID 
		and m.AID!=0
		and a.AID=@key
	order by m.MTime desc
else if(@by='T')
	select
		m.MID,
		m.MName,
		m.MUrl
	from
		Media  m,
		Article a
	where 
		m.AID=a.AID
		and m.AID!=0
		and a.ATitle like '%'+@key+'%'
	order by m.MTime desc
go

--执行测试
--exec GetArticileImg 'N','100001'
go


--删除文章图片
--输入：附件名称
--输出：true/false 字符串
create proc DelArticleImage
(
@url nvarchar(200)=''
)
as
begin transaction
delete from Media where MUrl=@url
if(@@ERROR=0)
begin
	commit transaction
	select 'true'
end
else
begin
	rollback transaction
	select 'false'
end
go

--执行测试
--exec DelArticleImage
go



---------------------------------------------
--标签管理相关操作
---------------------------------------------

--清空文章标签
--输入：文章ID
--输出：true/false字符串
create proc ClearTag
(
@id int
)
as
begin transaction
delete from Relation where AID=@id
if(@@ERROR=0)
begin
	commit transaction
	select 'true'
end
else
begin
	rollback transaction
	select 'false'
end
go

--执行测试
--exec ClearTag 10005
go


--添加标签及关系,没有则新增
--输入：表情名，文章id
--输出：true/false字符串
create proc AddTag
(
@tag nvarchar(50),
@aid int
)
as
begin transaction
declare @num int 
set @num=0
select @num=TNum from Tag where TName=@tag
if(@num=0)
begin
	insert into Tag values(@tag)
	select @num=TNum from Tag where TName=@tag
end
insert into Relation values(@aid,@num)
if(@@ERROR=0)
begin
	commit transaction
	select 'true'
end
else
begin
	rollback transaction
	select 'false'
end
go

--执行测试
--exec AddTag '测试标签',10000
go


--获取指定文章标签
--输入：文章ID
--输出：文章标签列表
create proc GetTagById
(
@aid int
)
as
select 
	t.TName 
from 
	Relation r,
	Tag t 
where 
	r.TNum=t.TNum 
	and r.AID=@aid
go

--执行测试
--exec GetTagById 10006
go


--标签表操作删改
--输入：操作类型(D/U)，标签ID，新的标签名 默认为空
--输出：true/false字符串
create proc MgrTag
(
@type char,
@id int=0,
@name nvarchar(100)=''
)
as
begin transaction
if(@type='D')
begin
	delete from Relation where TNum=@id 
	delete from Tag where TNum=@id 
end
else if(@type='U')
	update Tag set TName=@name where TNum=@id
if(@@ERROR=0)
begin
	commit transaction
	select 'true'
end
else
begin
	rollback transaction
	select 'false'
end
go

--执行测试
--exec MgrTag 'D',110
--exec MgrTag 'U',109,'测试标签'
go


--获取标签列表内容
--输入：无
--输出：所有文章标签信息
create proc GetTagList
as
select 
	TNum,
	TName,
	(
	select
		 count(AID)
	from 
		Relation r 
	where 
		t.TNum=r.TNum
	)
	as
	ACount
 from 
	Tag t
go

--执行测试
--exec GetTagList
go



---------------------------------------------
--内容分类管理相关操作
---------------------------------------------

--获取分类列表
--输入：无
--输出：内容分类列表
create proc GetCategory
as
select * from Category order by CID asc
go

--执行测试
--exec GetCategory
go


--分类表操作增删改
--输入：操作类型(I/D/U)，分类ID，新的分类名 默认为空
--输出：true/false字符串
create proc MgrCategory
(
@type char,
@id int=0,
@name nvarchar(100)=''
)
as
begin transaction
if(@type='I')
	insert into Category values(@name)
else if(@type='D')
	delete from Category where CID=@id 
else if(@type='U')
	update Category set CName=@name where CID=@id
if(@@ERROR=0)
begin
	commit transaction
	select 'true'
end
else
begin
	rollback transaction
	select 'false'
end
go

--执行测试
--exec MgrCategory 'I',default,'SQL测试'
--exec MgrCategory 'D',105
--exec MgrCategory 'U',106,'SQL测试'
go


--获取分类列表内容
--输入：无
--输出：网站分类信息
create proc GetCategoryList
as
select 
	CID,
	CName,
	(
		select
			 count(AID)
		from 
			Article a 
		where 
			a.CID=c.CID
	)
	as
	ACount
 from 
	Category c
go

--执行测试
--exec GetCategoryList
go



---------------------------------------------
--前台信息获取相关操作
---------------------------------------------
	
--搜索结果集
--输入：关键字
--输出：搜索结果集
create proc SearchArticle
(
@key nvarchar(100)
)
as
select 
	a.AID,
	a.ATitle,
	a.AText,
	a.ATime,
	u.UName,
	c.CName
from
	Article a,
	Users u,
	Category c	
where
	a.ATitle like '%'+@key+'%' 
	or a.AText like '%'+@key+'%'
	and a.UNum=u.UNum
	and a.CID=c.CID
go

--执行测试
--exec SearchArticle '标题'
go


--获取指定文章详细信息
--输入：文章ID
--输出：文章详细信息
create proc GetArticleById
(
@id int
)
as
select 
	a.AID,
	a.ATitle,
	a.AText,
	a.ATime,
	u.UName,
	c.CName
from 
	Article a,
	Users u,
	Category c
where 
	a.AID=@id
	and a.UNum=u.UNum
	and a.CID=c.CID
go

--执行测试
--exec GetArticleById 10001
go


--获取文章邻居页ID
--输入：当前页文章ID
--输出：Prev 上一页文章ID Next 下一页文章ID
create proc GetBrother
(
@id int
)
as
declare @prev int,@next int 
set @next=0
set @prev=0
select top 1 @prev=AID from Article where AID<@id order by AID desc
select top 1 @next=AID from Article where AID>@id order by AID asc
if(@prev=0)
	select @prev=max(AID) from Article
if(@next=0)
	select @next=min(AID) from Article
select @prev  as Prev ,@next as Next
go

--执行测试
--exec GetBrother 10000
go
