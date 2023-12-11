# API for job-board
This custom API is created to display job/user/company/resume data and be able to manipulate with it.

### Programming language
.NET 6 
### ORM
This project is using entity-framework
### Models
In total there are 8 custom entities to get/set data in database
### Authentication Authorization
Project is using JWT tokens to authenticate and authorize users to the content all unauthorized users will get error message and content won't be displayed.
### Roles
There are in total 3 different roles
### Resources
There are 4 main resources - users, companies, jobs, resumes
### Validation
There are some methods being validated inside this back-end server
### Repositories
All data is being managed there
### Custom DTO
Project includes custom models to manipulate with database data through controller using repository (entity-framework)
