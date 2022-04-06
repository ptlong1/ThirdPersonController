mergeInto(LibraryManager.library, {
	ConnectRoom: function(str)
	{
		console.log("ConnectRoom function library");
		connectRoom(Pointer_stringify(str));
	}
});