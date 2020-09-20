# MSTD-Backend

## About
This project is an ASP.NET Core 3.1 API project that replaces a (depracted) all-in-one [solution's](https://github.com/aivarasatk/MultiSourceTorrentDownloader) torrent search related logic. 
Corresponding [UI project](https://github.com/aivarasatk/MSTD-UI).
<br>
At the moment backend supports original proxies and sites. Having an API deliver sources to UI allows for flexible addition and removal of new and obsolete sources.

## Deployment
Solution contains an app.yaml which is used for google cloud deployment which uses a standard Dockerfile (also in the solution).

## Documentation
When API is up it can be located at [appspot](https://mstd-backend.ew.r.appspot.com/swagger)
<br>
Solution contains four main endpoints:
- /sources for retrieving states for available sources
- /torrents search endpoint with additional filter parameters
- /magnet allows to retrieve only a magnet for specified torrent
- /description provides an html description parsed from the hosting website

## Tests
TBD

# License
[MIT license](license.txt)
