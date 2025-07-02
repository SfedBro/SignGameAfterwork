Week \#4

## Post-MVP optimization and automation 

### **Deliverables**

* Code repository: [GitHub](https://github.com/IU-Capstone-Project-2025/SignGame)

* Project TaskBoard: [Trello](https://trello.com/b/g98QWgRE/sign-game)

* Interactive board of ideas: [Miro](https://miro.com/welcomeonboard/NjllanVudnhUd2Fhd3RGQUpCMlN0S3d2Nm9SakkrNzI1YVhsK0VKYmZpQkR6Titjc2xycjRyNnpYRTNGRTlvNyt5anpZa3R4TkZVUEdwNjIwdDVTcjdqQksyeUJBbTcreDg3cXNHWllsZFk2VWlhSHRvTTJ2aU5uU3BuR2hvRG5NakdSWkpBejJWRjJhRnhhb1UwcS9BPT0hdjE=?share_link_id=131423753479)

* CI/CD configuration: [web version](https://github.com/IU-Capstone-Project-2025/SignGame/blob/main/.github/workflows/lint_build_deploy.yml) and [OS autobuild](https://github.com/IU-Capstone-Project-2025/SignGame/blob/main/.github/workflows/platform_builds.yml)

* Deployed game version: [deploy](https://iu-capstone-project-2025.github.io/SignGame/WebGL/)

In this week we should update our project to ensure quality through testing, automate processes, and prepare for future deployment.

## Testing and QA

### Testing strategy 

* Base testing system:

  1. Feature developer is **locally testing** a new game update and pushes

  2. In case of success, **pushes** changes to the optional branch

  3. Makes **pull request** to the main branch

  4. **Linting** of the incoming code occurs

  5. In case of success, **pushes** changes to the [main branch](https://github.com/IU-Capstone-Project-2025/SignGame)

* Additionally system for ML model:

  1. Dataset **creation**
 
  2. Dataset is **divided** into teaching, validation and test

### Evidence of test execution

#### C# Linter log and result 

![linter](https://github.com/IU-Capstone-Project-2025/SignGame/blob/reports/assets/checkpoint.gif)

#### WebGL Lint & Build & Deploy result

![webgl](https://github.com/IU-Capstone-Project-2025/SignGame/blob/reports/assets/checkpoint.gif)

#### OS build 

![os](https://github.com/IU-Capstone-Project-2025/SignGame/blob/reports/assets/checkpoint.gif)

## CI/CD

### Description

CI/CD s a technology for testing and delivery of new features/modules of the project being developed. We use this technology and build special *.yml* files, which can:

- Lint all C# code

- Build game for any existing operationg system

- Deploy web version as a website

### Tools used

- **GitHub Pages** - GitHub module which can deploy project as a [website](https://iu-capstone-project-2025.github.io/SignGame/WebGL/)

- **csharpier** - C# linter library

- [Game.ci](https://game.ci) builder module - Feature to build Unity project in any OS

### Challenges faced

1. Too hard to manage C# code issues

  * Since C# is a OOP-structured language with syntax structure.
  
  * Even if the code is written bad, but in compliance with the syntax rules, that won't count as an issue.

2. Project delivery (CI)

  * Since project total size is too big to *game.ci builder*, there is need to minimize it with *archiving*

  * Without *Decompression fallback* setting, there is no possibility to unpack our project

3. Project deployment (CD)

  * Due to the fact that we badly worked out the window parameters, depending on the browser, the game UI can change 


### CI/CD configuration files

#### Web lint & build & deploy

[web version](https://github.com/IU-Capstone-Project-2025/SignGame/blob/main/.github/workflows/lint_build_deploy.yml) 

#### Windows, MacOS and Linux builds

[OS autobuild](https://github.com/IU-Capstone-Project-2025/SignGame/blob/main/.github/workflows/platform_builds.yml)



## Confirmation of the code's operability

We confirm that the code in the main branch:

* [✓] In working condition.
* [✓] Run via docker-compose (or another alternative described in the README.md).

*Innopolis University    |   Capstone project    |   Summer 2025*
