# Petbook :dog2:

`Petbook` is a unique social media platform specifically designed for pet owners to connect, share, and engage with fellow pet enthusiasts. With a focus on pets, this app provides a dedicated space where users can showcase their furry friends, exchange stories, seek advice, and build a vibrant community of pet lovers.


### Technical Implementation  :wrench::
The app is developed using the `C#` programming language and the `ASP.NET Core MVC` framework, providing a scalable and secure architecture. `SQL Server` is utilized as the database management system, ensuring efficient storage and retrieval of user data.

### Project made by:

- Neaga-Budoiu Maria
- Panait Ana-Maria
- Teodorescu George Tiberiu

### Requirements:
-> Install Croppie
`npm install croppie`


## User stories :raising_hand:
    1. As a user, I want to share photos of my pet.
    2. As a user, I want to be able to see the number of likes on a post and also the list of users that liked it.
    3. As a user, I want to be able to comment on other people's posts.
    4. As a user, I want to be able to like a post and to be notified when my posts are liked.
    5. As a user, I want to customize my pet's profile.
    6. As a user, I want to be able to like comments and to delete comments on my posts.
    7. As a user, I want to be able to edit my comments on other's posts.
    8. As a user, I want to be able to share stories about my pet.
    9. As a user, I want to be able to talk about subjects related to taking care of pets (tips & tricks).
    10. As a user, I want to be able to delete my posts.
    11. As a user, I want to be able to put tags on my stories so that people can find them easier.
    12. As a user, I want to search by tags/title posts.
    13. As a user, I want to be able to search other pets accounts.
    14. As a user, I want to be able to create multiple pet accounts.
    15. As a user, I want to be able to follow other people and see their pet's posts.
    16. As an admin, I want to be able to delete inappropriate posts.
    17. As an admin, I want to be able to ban users that have suspicious/inappropriate activity.
    18. As a visitor, I want to be able to create an account for my pets.

## Backlog creation :page_with_curl:
For the backlog creation we used [Trello platform](https://trello.com/b/4mMTerl6/petbook).
![image](https://github.com/anamariapanait10/Petbook/blob/main/Trello.png)

## Diagrams (ERD, UML)

![image](https://github.com/anamariapanait10/Petbook/blob/main/ERD.jpg)
![image](https://github.com/anamariapanait10/Petbook/blob/main/UML.png)

## Source control (branch creation, merge/rebase, pull requests)

- For each implemented functionality we used separate branches that we merged in `main` branch after
the functionality was ready. You can find the list of all branches created [here](https://github.com/anamariapanait10/Petbook/branches).
![image](https://github.com/anamariapanait10/Petbook/blob/main/Branches.png)
- The list of all commits can be found [here](https://github.com/anamariapanait10/Petbook/commits/main).

## Automated tests
- For testing we implemented several unit tests for pets controller to check that the CRUD operations work correctly.

![image](https://github.com/anamariapanait10/Petbook/blob/main/automated_tests.jpeg)

## Bug report
We have encounter this bug: The Redirect to ShowNotifications page was not working and the 
following error appeared:

![image](https://github.com/anamariapanait10/Petbook/blob/main/BugReport1.png)

The ploblem was that the method from the controller was private instead of public, therefore couldn't be accessed from extern context.
![image](https://github.com/anamariapanait10/Petbook/blob/main/BugReport2.png)

## Refactoring, code standards
We used refactoring to rename variables and function names as suggestive as possible. We used the following coding style
for the code: https://github.com/dotnet/runtime/blob/main/docs/coding-guidelines/coding-style.md.

## Comments
We used many comments in this project to clearly explain the implementation.

## Design patterns
-> **Model-View-Controller (MVC) Pattern:**

The MVC pattern is the core architectural pattern used in `ASP.NET Core MVC` applications. It separates the application into three main components:

- Model: Represents the data and business logic of the application.
- View: Displays the user interface and interacts with the user.
- Controller: Handles user input, updates the model, and selects the appropriate view to render.

-> **Observer Design Pattern**

The observer design pattern is used by default in MVC projects. 
Its goal is to create this one-to-many relationship between the subject and all of the observers waiting for data so they can be updated 
(relationship between a model and all the views associated with it). 

To show its implementation in our project, we consider, for example, the `BlogPost` model, which is the subject, and the operations associated with a blog post: 
- indexing all
- adding a new post
- deleting 
- editing

These operations are implemented using special views, which are the observers for the subject. Anytime the state of the BlogPost model changes, 
all the observers will be notified and updated instanly.

## GitHub Copilot/chatGPT
We used chatGPT to generate blog posts, post descriptions and the home page welcome message.
![image](https://github.com/anamariapanait10/Petbook/blob/main/home_page_message.png)
![image](https://github.com/anamariapanait10/Petbook/blob/main/blogposts.png)

## Video demo
The demo of the app can be found [here]().
