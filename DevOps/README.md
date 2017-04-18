# DevOps In Azure
*Presented by Nick Martin*

To review the concepts, refer to the associated [presentation slides](Presentation.pptx).

This lab shows the following key concepts:
- Creating a Delivery Pipeline in Visual Studio Team Services (VSTS)
- Resource management with Azure Resource Manager (ARM) templates
- Configuration management with VSTS and ARM templates for multiple environments
- Application Lifecyle Management (ALM)

An important concept in the Ops part of DevOps is operational support. Azure provides a large number of mechanisms for logging, monitoring, and tracking telemetry, but these are out of scope for this lab.

**Lab Tasks**:
1. [Setting Up VSTS](#1-Setting-Up-VSTS)
1. [Setting Up the Delivery Pipelines](#2-Setting-Up-the-Delivery-Pipelines)
1. [Adding a New Environment](#3-Adding-a-New-Environment)
1. [Adding New Resources and Deployable](#4-Adding-New-Resources-and-Deployable)

## 0. Prerequisites

- **A Windows machine** - this is due to the Visual Studio requirement.
<br />*NOTE:* If not on Windows, a similar but simpler [Microsoft-provided lab](https://github.com/Microsoft/TechnicalCommunityContent/blob/master/DevOps/DevOps/Session%203%20-%20Practical%20DevOps/HOL%203.2%20-%20VotingApp%20Full%20Continuous%20Integration%20Deployment/HOL%203.2.md) can get you started, and you can review this lab content for more advanced concepts such as handling multiple environments with configuration management.
- **An internet connection** - during the bootcamp, instructions will be provided for you to get online.
- **An active Azure subscription** - if you haven't already, you can get a [free trial](https://azure.microsoft.com/en-us/free/), otherwise the bootcamp organizers may be able to provide you with free credits.
- **An active VSTS Account** - [create one if needed](https://www.visualstudio.com/en-us/docs/setup-admin/team-services/sign-up-for-visual-studio-team-services). Note the instructions below assume a Git repository, but this lab can be also be completed with a Team Foundation Version Control repository.
<br />*NOTE:* If possible, use the same login credentials you use for your Azure subscription for the VSTS account. This enables direct integrate of your Azure subscription in VSTS. Otherwise, additional steps are necessary to connect them.
- **Visual Studio 2017** - the free [Community Edition](https://www.visualstudio.com/downloads/) works fine.
<br />*NOTE:* You do NOT have to be a developer to complete this lab. All the application logic is written for you.
<br />*NOTE:* Earlier versions should work, but has not been tested and may require additional setup.
- **Git tools** - [download](https://git-scm.com/downloads) directly, or optionally installed with Visual Studio.
- **Microsoft SQL Server Express** (*Recommended*) - for running the application locally.

## 1. Setting Up VSTS

In order to keep things organized within your account, lets create a new Team Project.

### 1.a Creating the Team Project


1. Login to your VSTS account at {your-account-name}.visualstudio.com
1. Creat a new team project
![New Project button](images/vsts-new-project-button.png)
1. Name your project **GAB2017** and, use **Git** for the version control repository, choose the **Scrum** process template (purely for preference), and click **Create**
1. In order to use the Git CLI, click **Generate Git credentials**
![Generate Git credentials button](images/generate-git-credentials.png)
1. Enter credentials and **Save Git Credentials**

### 1.b Adding Work Items

At this point your Team Project is ready to use, but lets add some work items to demonstrate the ALM capabilities of VSTS and some ways that the Delivery Pipeline ties into them.
<br />*NOTE:* There is a LOT more to the ALM capabilities than what is shown in this lab.

1. Go to the **Work** section of your new Team Project via the top-level navigation bar
![Work menu item](images/vsts-work-menu-item.png)
1. Add the following work items:
    1. **Create basic website**
    1. **Add Web Job background task**
    1. **Record feedback to database**
<br />![Creating work items](images/add-backlog-items.png)

For the purpose of this lab, don't bother putting any details into these work items other than the title.

### 1.c Preparing the Source Control Repository

The project sources are currently in this GitHub repository. VSTS allows you to drive a Delivery Pipeline from a GitHub repository, but this would require you to fork the repository in order to have access to push updates to trigger the Continuous Integration (CI) process.

Instead, we will use the Git repository within the VSTS repository so will copy the sources over.
<br />*NOTE:* You can also push existing repositories into VSTS rather than the below method.

1. First, if you haven't already, [clone this GitHub repository](https://help.github.com/articles/cloning-a-repository/) to your machine
1. Go back in VSTS, and return to the **Code** section
![Work menu item](images/vsts-code-menu-item.png)
1. Copy the HTTPS url to the Git repository
![Copy repository path button](images/clone-repository.png)
1. In your Git command prompt of choice and navigate to where you want to clone the VSTS repository
1. Clone the repository with command `git clone <paste-HTTPS-url-copied-earlier>`
<br />*NOTE:* Git will give you a warning that you've cloned an empty repository, we will ignore the warning.
1. Move into the cloned repository folder with command `cd GAB2017`
1. In Windows Explorer copy the entire contents within the GitHub repository's **DevOps/begin** folder into the VSTS repository **GAB2017** folder
1. Back in the command prompt, commit and push all sources using the following commands:
```
git add .
git commit -m "Adding sources"
git push origin master
```

Feel free to open the **Feedback.sln** file in Visual Studio and run the website. This is a very simple page that has a feedback form. Submitted feedback is stored in SQL and all saved history is listed back to the user. When running locally, the 

## 2. Setting Up the Delivery Pipeline

A [Delivery Pipeline](https://devops.com/continuous-delivery-pipeline/) is the core pattern that underpins Continuous Delivery. Creating and managing this pipeline is a common task of DevOps teams.

### 2.a Creating the Build Definition

A Delivery Pipeline always starts with a build being triggered by some change in the system. This is refered to as a Continuous Integration (CI) build.

Microsoft is currently rolling out a new build definition editor. If prompted, opt in to the new editor in order to have an experience consistent with these instructions.

1. Go back in VSTS, and go to the **Build & Release** section
![Build & Release menu item](images/vsts-build-release-menu-item.png)
1. Click **New** to create a new build definition
<br />![New build definition button](images/new-build-definition.png)
1. In the list, select **Visual Studio** (should be selected by default) and click **Next**
1. In the next screen, validate the following settings:
   * Repository source should be **GAB2017 Team Project**
   * Repository should be **GAB2017**
   * Default branch should be **master**
   * The **Continuous integration (build whenever this branch is updated)** option should be checked *(not default)*
   * Default agent queue should be **Hosted VS2017** *(not default)*
   * Select folder should be **\\**
1. Click **Create**
<br />*NOTE:* This didn't actually create a build definion, but rather a template for a build definition. In the following steps you configure the build definition and save it.
1. Select the **Build solution \*\*\\*.sln** build task
<br />![Build solution build task](images/build-solution-build-task.png)
1. Add the following to the MSBuild Arguments field: `/p:DeployOnBuild=true /p:PublishProfile=Release /p:WebPublishMethod=Package /p:PackageAsSingleFile=true /p:SkipInvalidConfigurations=true /p:PackageLocation=$(Build.ArtifactsStagingDirectory)\Feedback.Web.zip`
<br />*NOTE:* These arguments tell Visual Studio to package the build outputs into clean and convenient little artifacts for release to Azure.
<br />![MSBuild arguments field](images/msbuild-arguments.png)
1. Select the **Test Assemblies \*\*\\$(BuildConfiguration)\\\*test\*.dll;-:\*\*\\obj\\\*\*** build task
1. Expand **Advanced Execution Options** and set the VSTest version to **Latest**
<br />![VSTest version update](images/vstest-version-setting.png)
1. Delete the **Copy Files to: $(build.artifactstagingdirectory)** build task
<br />*NOTE:* We already told MSBuild to deploy the built packages to this location.
<br />![Copy artifacts build task](images/copy-artifacts-build-task.png)
1. Click **Add build step** above the task list
1. In the **Build catalog** window, select **Utility**, find **Publish Build Artifacts** and click **Add**
<br />![Add publish build artifacts task](images/add-publish-build-artifacts-task.png)
1. Close the **Build catalog**
1. Select the newly-added task (it will be in red text and on the bottom of the task list)
1. Publish the ARM folder from the source repository to an artifact named **arm**, with Artifact type **Server**
<br />*NOTE:* In order for the Release Definition to have access to our ARM templates, we need to publish it from source in an artifact. Release definitions don't have access to the source repository like build definitions do.
<br />![Configure publish arm folder build task](images/publish-arm-folder.png)
1. Click **Save**
<br />![Save build definition button](images/save-build-definition.png)
1. Enter **GAB2017 CI** in the Name field and click **OK**
<br />![Finalize CI build](images/finalize-ci-build.png)

Your CI build is now created and any future commits into the source repository will automatically trigger the build to run. Lets test it out to make sure we don't have errors. Click **Queue new build** and click **OK** at the prompt. 
<br />![Queue new build button](images/queue-new-build.png)

The build should complete successfully.

### 2.b Creating the Release Definition

1. While in the **Build & Release** section of VSTS, click the **Releases** tab
![Releases menu item](images/releases-menu-item.png)
1. Because you don't have any release definitions, you're only option is to click **New definition**
1. Select the **Azure App Service Deployment** option and click **Next**
1. Check the **Continuous deployment (create release and deploy whenever a build completes)** option and click **Create**
<br />*NOTE:* Again, this didn't actually create the release definition but got us started with a template.
1. ........................................................
1000. Click **Deploy** to execute the release. This will take some time to provision all the resources in your Azure subscription.

While you wait, take a moment to examine the ARM template. Open file **ARM/template.json** in the Visual Studio solution. Since this was all done for you, it is helpful to know where you can get information about what goes into the template. The links under the **Template format** columns [here](https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-manager-supported-services) are a great resource for creating and expanding your own ARM templates from scratch.

You can explore the basics of ARM templates on your own, but for the purpose of this lab there are number of specifics I want to call attention to.

- The region (paramater **datacenter_region**) is set to **westus**. This is because later we will use a preview feature of Cognitive Services that is only available in that region.
- Any parameter that needs to be globally unique via DNS is actually a prefix, which we then append a [uniqueString](https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-template-functions#uniquestring) to. This is to prevent people in the lab from conflicting with each other as resources are created. Refer to the variable **app_site_name** as an example.
- The App Service plan is scaled at **B1** instead of using the free tier so we can use [Always On in order to run a Web Job that will be added later continuously](https://docs.microsoft.com/en-us/azure/app-service-web/web-sites-create-web-jobs#a-namecreatescheduledcronacreate-a-scheduled-webjob-using-a-cron-expression).

## 3. Adding a New Environment

1. tbd...

## 4. Adding New Resources and Deployable

It is great that we got our website online. But this isn't much of a robust system. Over the lifecycle of a project it is very common for additional resources and services to be added. In this section we will add Cognitive Text Analysis to calculate a sentiment score for each piece of feedback, a Web Job (in its own App Service, within the same App Service Plan) that does the behind-the-scenes work of updating the feedback with the sentiment score, and a storage account for use by the Web Jobs SDK.

### 4.a Enabling Cognitive Services APIs

First, we need to make sure your Azure subscription enables the creation of Cognitive Services since it is currently a preview feature.

1. Log in to the [Azure Portal](https://portal.azure.com/).
<br />![Add Cognitive Services API navigation](images/add-cognitive-services.png)
1. Click the search result: **Cognitive Services APIs (preview)**
1. Click **Create**
1. Type anything that would satisfy validation into the **Account name**, select **Text Analytics API (preview)** as the **API type**, select **F0** as the **Pricing tier**, and enter anything that would satisfy validation into the **Resource group**.
<br />*NOTE:* The location should default to West US, otherwise change it.
1. Click **Account create disabled**
<br />![Enable Cognitive Services account creation](images/enable-cognitive-services.png)
<br />*NOTE:* If you don't see this - great! You already have this enabled and can stop right here. There is no need to need to create the resource.
1. Click **Enable** and **Save**
1. You should receive a notification that account creation is enabled. You don't actually have to follow-through with creating the resource in the portal. We will have the ARM template do this for us!

Notice we didn't actually create anything while in the portal. The best DevOps teams very rarely do anything manually through the portal, but opting-in to preview features is one of those rare exceptions.

### 4.b Updating the ARM Template

1. tbd...

- List keys...
- Not using free tier for one environment...

### 4.c Adding the Web Job Project

1. tbd...
