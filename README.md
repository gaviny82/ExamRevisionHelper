# PastPaperHelper
This tool is built for downloading and managing CIE past papers

# Roadmap
We are currently in the beta phase. More functions including are in progress.

| Version |       Date      |
|---------|-----------------|
|RC       |End of August    |
|GA       |September        |


# Changelog

## Beta 0.3

### New
- Subject selection and subscription
- View exam series and papers on Files page

### Changes
- Subject data is now updated every 7 days
- Data loaded from local files instead of servers on startup
- Performance Improvements
- New data structure for storing papers

### Known Issues
- Virtualization of the ItemsControl on Files page is not working
- Crashes when selecting a subject that is added without restarting (hot reload of paper repos should be added)

## Beta 0.2

### New
- View subject subscribed on download page
- Fetch data from web servers to update subject info

## Beta 0.1 - Initial release
- Search in paper downloaded
- Select a folder as the source