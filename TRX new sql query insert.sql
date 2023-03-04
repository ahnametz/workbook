delete PlaylistTracks where PlaylistId = 400
go
delete Playlists where PlaylistId = 400
go
set identity_insert Playlists on
insert into Playlists(PlaylistId, Name, UserName) values(400,'Hansenb400','HansenB')
set identity_insert Playlists off
go

insert into PlaylistTracks(PlaylistId,TrackId,TrackNumber) values(400,12,4)
insert into PlaylistTracks(PlaylistId,TrackId,TrackNumber) values(400,122,2)
insert into PlaylistTracks(PlaylistId,TrackId,TrackNumber) values(400,54,1)
insert into PlaylistTracks(PlaylistId,TrackId,TrackNumber) values(400,1,3)
insert into PlaylistTracks(PlaylistId,TrackId,TrackNumber) values(400,11,5)
insert into PlaylistTracks(PlaylistId,TrackId,TrackNumber) values(400,111,7)
insert into PlaylistTracks(PlaylistId,TrackId,TrackNumber) values(400,200,6)
go