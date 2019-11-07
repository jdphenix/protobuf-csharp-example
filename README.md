# protobuf-csharp-example

Just my demo project demonstrating differences between serialization sizes of
JSON versus protobuf, both uncompressed and compressed with deflate. 

My own conclusions really boil down to processing time versus space. Space is negligibly
different. From the data below, compressing JSON versus compressing protobuf isn't
*that* different, so I'd make a decision on which to use based on processing time. 

I bet that JSON processing will be flying with the new .NET Core 3.0 JSON API, but 
obviously I don't really know until I actually test. 

## Output sizes

         1624 25b90d78-3b94-4356-a72c-265d4dea6eaf.100.compressed.dat
         1705 25b90d78-3b94-4356-a72c-265d4dea6eaf.100.compressed.json
         2318 25b90d78-3b94-4356-a72c-265d4dea6eaf.100.dat
         6619 25b90d78-3b94-4356-a72c-265d4dea6eaf.100.json
          214 38af6f2a-9426-4642-aa1f-2a91a61e0555.10.compressed.dat
          251 38af6f2a-9426-4642-aa1f-2a91a61e0555.10.compressed.json
          236 38af6f2a-9426-4642-aa1f-2a91a61e0555.10.dat
          667 38af6f2a-9426-4642-aa1f-2a91a61e0555.10.json
    127359416 8aa39f6e-d7c5-4edb-a9a5-04a2b821d7e7.10000000.compressed.dat
    142347211 8aa39f6e-d7c5-4edb-a9a5-04a2b821d7e7.10000000.compressed.json
    230879578 8aa39f6e-d7c5-4edb-a9a5-04a2b821d7e7.10000000.dat
    660898029 8aa39f6e-d7c5-4edb-a9a5-04a2b821d7e7.10000000.json
     12736743 b6f3fe46-44bd-48d8-8209-f69682e54a13.1000000.compressed.dat
     14235987 b6f3fe46-44bd-48d8-8209-f69682e54a13.1000000.compressed.json
     23086826 b6f3fe46-44bd-48d8-8209-f69682e54a13.1000000.dat
     66088812 b6f3fe46-44bd-48d8-8209-f69682e54a13.1000000.json
      1275057 c36e6fa7-3b05-4f5a-b473-114c93db447a.100000.compressed.dat
      1423686 c36e6fa7-3b05-4f5a-b473-114c93db447a.100000.compressed.json
      2308654 c36e6fa7-3b05-4f5a-b473-114c93db447a.100000.dat
      6608815 c36e6fa7-3b05-4f5a-b473-114c93db447a.100000.json
        14201 c3bc602a-57de-4abd-8e5b-2b8c049ad712.1000.compressed.dat
        15012 c3bc602a-57de-4abd-8e5b-2b8c049ad712.1000.compressed.json
        23116 c3bc602a-57de-4abd-8e5b-2b8c049ad712.1000.dat
        66127 c3bc602a-57de-4abd-8e5b-2b8c049ad712.1000.json
       129069 ff66951e-a96a-4ed0-92c9-b7695f3a0230.10000.compressed.dat
       143091 ff66951e-a96a-4ed0-92c9-b7695f3a0230.10000.compressed.json
       231089 ff66951e-a96a-4ed0-92c9-b7695f3a0230.10000.dat
       661115 ff66951e-a96a-4ed0-92c9-b7695f3a0230.10000.json
