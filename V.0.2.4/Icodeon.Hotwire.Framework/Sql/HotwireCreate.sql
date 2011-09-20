use hotwireUnitTest

-- drop table --

if exists (select * from  sys.objects where object_id = object_id (N'[dbo].[SimpleMacHistory]'))
drop table [dbo].[SimpleMacHistory]
GO 

-- create table --
create table [dbo].[SimpleMacHistory]
(
	[Salt] [uniqueidentifier] NOT NULL,
	[Url] [nvarchar](200) NOT NULL,  	
	[Created] [datetime] NOT NULL,
	-- Expires is when the timestamp expires and it is safe to remove this entry such that replay is not possible.
	[Expires] [datetime] NOT NULL,
	constraint [PK_SimpleMacHistory_Salt] primary key clustered( [Salt] asc )
) 

GO

-- create indexes --
create nonclustered index [IX_SimpleMacHistory_ByExpires] on [dbo].[SimpleMacHistory] (	[Expires] asc )

GO 

