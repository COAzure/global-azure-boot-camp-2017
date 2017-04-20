# DevOps In Azure
*Presented by Nick Martin*

To review the concepts, refer to the associated [presentation slides](Presentation.pptx).

This lab shows the following key concepts:
- Creating a Delivery Pipeline in Visual Studio Team Services (VSTS)
- Resource management with Azure Resource Manager (ARM) templates
- Configuration management with VSTS and ARM templates for multiple environments

An important concept in the Ops part of DevOps is operational support. Azure provides a large number of mechanisms for logging, monitoring, and tracking telemetry, but these are out of scope for this lab.

**Lab Tasks**:
1. [Setting Up VSTS](#1-setting-up-vsts)
1. [Setting Up the Delivery Pipelines](#2-setting-up-the-delivery-pipeline)
1. [Adding a New Environment](#3-adding-a-new-environment)
1. [Adding New Resources and Deployable](#4-adding-new-resources-and-deployable)

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

### 1.b Preparing the Source Control Repository

The project sources are currently in this GitHub repository. VSTS allows you to drive a Delivery Pipeline from a GitHub repository, but this would require you to fork the repository in order to have access to push updates to trigger the Continuous Integration (CI) process.

Instead, we will use the Git repository within the VSTS repository so will copy the sources over.
<br />*NOTE:* You can also push existing repositories into VSTS rather than the below method.

1. First, if you haven't already, this GitHub repository to your machine ([instructions](https://help.github.com/articles/cloning-a-repository/))
1. Go back in VSTS, and return to the **Code** section
![Work menu item](images/vsts-code-menu-item.png)
1. Copy the HTTPS url to the Git repository
![Copy repository path button](images/clone-repository.png)
1. In your Git command prompt of choice (such as PowerShell) and navigate to where you want to clone the VSTS repository
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

Microsoft is currently rolling out a new build definition editor. The screenshots in these instructions are for the old build definition editor. If you opt in to the new UI the look & feel will vary but the steps should approximately match. You can control which editor is in use via the **Preview features** panel.
<br />![Preview features navigation](images/vsts-preview-features.png)

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
1. Add the following to the MSBuild Arguments field: `/p:DeployOnBuild=true /p:PublishProfile=Release /p:WebPublishMethod=Package /p:PackageAsSingleFile=true /p:SkipInvalidConfigurations=true /p:PackageLocation=$(Build.StagingDirectory)`
<br />*NOTE:* These arguments tell Visual Studio to package the build outputs into clean and convenient little artifacts for release to Azure. There was also a bit of setup done for you in creating the "Release" publish profile, which should be familiar to anyone who is used to ASP.NET projects and is not specific to this lab.
<br />![MSBuild arguments field](images/msbuild-arguments.png)
1. Select the **Test Assemblies \*\*\\$(BuildConfiguration)\\\*test\*.dll;-:\*\*\\obj\\\*\*** build task
1. Expand **Advanced Execution Options** and set the VSTest version to **Latest**
<br />![VSTest version update](images/vstest-version-setting.png)
1. Select the **Copy Files to: $(build.artifactstagingdirectory)** build task
1. Change the **Source Folder** to `$(Build.StagingDirectory)` and **Contents** to `**\*`
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

### 2.b Linking VSTS to your Azure Subscription

VSTS needs to have access to your Azure subscription in order to automate the tasks of provisioning resources and deploying your applications.

Follow [these instructions](https://blogs.msdn.microsoft.com/visualstudioalm/2015/10/04/automating-azure-resource-group-deployment-using-a-service-principal-in-visual-studio-online-buildrelease-management/). If used the same credentials as you have for your Azure subscription you should be able to follow the simple **Connect your Azure subscriptions to VSTS in 3 clicks** flow. If not, or you otherwise don't see your subscription, you will need to perform the more cumbersome flow under **Manual configuration**.
<br />*TIP:* Make sure you watch for popup blocker notifications to authenticate into Azure as part of the linking process.

### 2.c Creating the Release Definition

1. While in the **Build & Release** section of VSTS, click the **Releases** tab
![Releases menu item](images/releases-menu-item.png)
1. Because you don't have any release definitions, you're only option is to click **New definition**
1. Select the **Empty** option and click **Next** (the default templates are skimpy for what we need to do)
1. Check the **Continuous deployment (create release and deploy whenever a build completes)** option and click **Create**
<br />*NOTE:* Again, this didn't actually create the release definition but got us started with a template.
1. Rename the initial environment to `Prod`
<br />*TIP:* It may sound counter-intuitive, but I usually suggest to start a new solutions with production. It is amazing the number of last-minute issues that are avoided if from day-1 you know you can deploy to production. Also, like branching, the more environments you have the more complexity you have to manage throughout development. Since you *know* you have to have a production, start there, and add environments as you can justify their cost in complexity.
<br />![Rename environment](images/rename-environment.png)
1. Click the elipses (...) next to the enviornment name, and select the **Configure variables* context-menu item
1. Add variable name `dbPassword` with a value that meets the Azure database strength rules, click the padlock to encrypt and secure the variable value, and click **OK**
![Entering db password release variable](images/db-password-enviornment-variable.png)
1. Click **Add tasks**
1. Find task **Azure Resource Group Deployment**, click **Add**, click **Close**, and configure the task as follows:
   * Select the subscription you linked to VSTS
   * Action should be **Create or update resource group**
   * Enter `GAB2017` as the **Resource group**
   * Enter `West US` as the **Location**
   * Enter `$(System.DefaultWorkingDirectory)/GAB2017 CI/arm/template.json` as the **Template**
   * Enter `$(System.DefaultWorkingDirectory)/GAB2017 CI/arm/prod.json` as the **Template parameters**
   * Enter `-database_server_password $(dbPassword)` in **Override template parameters**
   <br />*NOTE:* As a general rule, you should never have secrets such as passwords in source control. Because the ARM template and parameter files are in source control, we must provide this parameter explicitly. The value `$(dbPassword)` refers to the VSTS environment variable created earlier.
   * Change the **Deployment mode** to **Validation only**
1. Add another **Azure Resource Group Deployment** task and configure it exactly as the first one, except set the **Deployment mode** to **Incremental**
<br />*NOTE:* Because provisioning resources can take some time, it is convenient to have a task to fail-fast out of the release pipeline if there is an obvious bug in the ARM template. This is the function of the **Validation only** task. **Incremental** ensures that the defined resources are always kept in-sync in Azure. In addition to initially creating the resource group, as new resources are added to the template they are added to Azure. This will be apparent later in this lab.
1. Add task **Azure PowerShell** and configure the task as follows:
   * Change **Azure Connection Type** to **Azure Resource Manager**
   * Select the subscription you linked to VSTS
   * Change **Script Type** to **Inline Script**
   * Add the following **Inline Script** text:
    ```
    $resourceGroup = 'GAB2017'
    $apps = Get-AzureRmWebApp -ResourceGroupName $resourceGroup
    foreach ($app in $apps)
    {
      $appName = $app.Name
      if ($appName.StartsWith('gab2017-site-'))
      {
        Write-Host ("Found app service: $appName")
        Write-Output ("##vso[task.setvariable variable=appServiceName;]$appName")
      }
    }
    ```
    <br />*NOTE:* The ARM template dynamically computes many of its resource names, including the DNS name of the app service to deploy to. This bit of script queries Azure for the full name of the desired app service based on a known prefix. The odd-looking `Write-Host` argument is how we store data to a VSTS variable for use by future tasks.
    <br />*TIP:* In the case of this lab, this is to make sure multiple people can work the lab at the same time without DNS conflicts, but this is actually a common practice in DevOps. The idea is to treat servers like cattle rather than pets...and you generally don't name your cattle in a meaningful name. 
1. Add task **Azure App Service Deploy** and configure the task as follows:
   * Select the subscription you linked to VSTS
   * Enter `$(appServiceName)` as the **App service name** (this is the variable captured by the PowerShell script)
   * Update the **Package or folder** to `$(System.DefaultWorkingDirectory)/GAB2017 CI/drop/Feedback.Web.zip`
1. Save the release definition
1. Click the **Release** pull-down button next to the now-disabled **Save** button, select **Create Release** and click **Create** in the modal dialog
1. To watch progress of the release:
   * Click the **Release-#** link
   <br />![Running release link](images/running-release-link.png)
   * Click the **Logs** tab
   <br />![Logs tab](images/release-logs-tab.png)

This will take some time to provision all the resources in your Azure subscription and deploy the application.

While you wait, take a moment to examine the ARM template. Open file **ARM/template.json** in the Visual Studio solution. Since this was all done for you, it is helpful to know where you can get information about what goes into the template. The links under the **Template format** columns [here](https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-manager-supported-services) are a great resource for creating and expanding your own ARM templates from scratch.

You can explore the basics of ARM templates on your own, but for the purpose of this lab there are number of specifics I want to call attention to.

- The region (paramater **datacenter_region**) is set to **westus**. This is because later we will use a preview feature of Cognitive Services that is only available in that region.
- Any parameter that needs to be globally unique via DNS is actually a prefix, which we then append a [uniqueString](https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-template-functions#uniquestring) to. This is to prevent people in the lab from conflicting with each other as resources are created. Refer to the variable **app_site_name** as an example.
- The App Service plan is scaled at **B1** instead of using the free tier so we can use [Always On in order to run a Web Job that will be added later continuously](https://docs.microsoft.com/en-us/azure/app-service-web/web-sites-create-web-jobs#a-namecreatescheduledcronacreate-a-scheduled-webjob-using-a-cron-expression).

By now the release should have finished successfully. You can visit the site at gab2017-site-{random-value}.azurewebsites.net. (look at the log output from the **Azure PowerShell** release task to know your full site name). The site may take a while to "wake up", but after it finishes you should be able to successfully submit feedback records.

## 3. Adding a New Environment

So, this is great, we have a production environment. We have a few users in our product, but we need to undertake new development efforts to add capabilities. We don't want our changes to go straight out to production and impact active users so we will create a development environment. Thanks to the ARM template, this is remarkably simple to do in Azure!

### 3.a Updating the Delivery Pipeline

1. Back in VSTS, edit your release definition
1. Click the ellipses (...) on the **Prod** environment and click **Clone environment** in the context menu.
1. Uncheck the **Trigger** option and click **Create**
1. Drag the newly-created enviornment above **Prod**, and rename it **Dev**
1. Click the ellipses for on the **Dev** environment and click **Configure variables**
1. Right now the database password for dev and prod is the same, which may not be a good idea, so enter a new strong password for **Dev**
*NOTE:* You have to "unlock" the value to change it. Be sure to lock it again to keep it secret.
1. Select the first release task and change the **Resource group** from "GAB2017" to `GAB2017Dev` and **Template parameters** to `$(System.DefaultWorkingDirectory)/GAB2017 CI/arm/dev.json`
1. Repeat the last step for the second release task
1. Select the third release task and update the first line of the **Inline Script** to assign `GAB2017Dev` to the **$resourceGroup** variable
*TIP:* The above 3 steps could have been avoided if we had used an environment variable for the resource group name. In that way, we wouldn't have to change the individual steps for each environment, instead only changing the environment variable to have a distinct value for each environment, similar to what we did above for the database password.
1. Click the **Triggers** tab
1. Edit the **Environment triggers** so that **Dev** is automatic and **Prod** is manual
![Updated environment triggers](images/environment-triggers.png)
1. Save the release definition

### 3.b Source Changes

1. Add file `dev.json` within the **ARM** folder on disk
1. Go to Visual Studio with the Feedback.sln open
*NOTE:* Solution folders in Visual Studio are completely logical. Unfortunately, if you want the file in the right place, you have to create it first then add it to Visual Studio.
1. In Solution Explorer, right click on the **ARM** folder and select *Add* > *Existing Item*, find **dev.json** and click **Add**
1. Copy all contents from **prod.json** to **dev.json**
1. For each parameter value, append `-dev` (for example, "gab2017-app-plan" becomes `gab2017-app-plan-dev`)
*NOTE:* This is only to more easily differentiate these resources visually by name. The ARM template will already randomly generate unique names for each.
1. Commit the sources to trigger the build and automatic deployment to the newly-created development environment using these commands:
```
git add .
git commit -m "Added dev environment ARM parameters file"
git push
```

It might surprise you how little effort in the code this was. No dealing with transforms. We let VSTS configuration management and the ARM parameter file handle all the [minor differences between environments](https://12factor.net/dev-prod-parity). 

## 4. Adding New Resources and Deployable

It is great that we got our website online. But this isn't much of a dynamic system. Over the lifecycle of a project it is very common for additional resources and services to be added. In this section we will add Cognitive Text Analysis to calculate a sentiment score for each piece of feedback, a Web Job (in its own App Service, within the same App Service Plan) that does the behind-the-scenes work of updating the feedback with the sentiment score, and a storage account for use by the Web Jobs SDK.

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

1. In Visual Studio, open **ARM/template.json**
1. Add the following JSON to the **parameters** object:
```
  "app_webjobs_name_prefix": {
    "type": "string"
  },
  "storageaccount_name_prefix": {
    "type": "string"
  },
  "storageaccount_sku": {
    "type": "object",
    "defaultValue": {
      "name": "Standard_RAGRS",
      "tier": "Standard"
    }
  },
  "cognetive_services_name": {
    "type": "string"
  },
  "cognetive_services_sku": {
    "defaultValue": "S1",
    "type": "string"
  }
```
1. Add the following JSON to the **variables** object:
```
  "app_webjobs_name": "[concat(parameters('app_webjobs_name_prefix'),'-',uniqueString(subscription().subscriptionId,resourceGroup().id))]",
  "storageaccount_name": "[concat(parameters('storageaccount_name_prefix'),uniqueString(subscription().subscriptionId,resourceGroup().id))]"
```
*NOTE:* This is what ensures unique DNS names across all participants of this lab.
1. Add the following JSON to the **resources** array:
```
  {
    "type": "Microsoft.Web/sites",
    "kind": "WebApp",
    "name": "[variables('app_webjobs_name')]",
    "apiVersion": "2015-08-01",
    "location": "[parameters('datacenter_region')]",
    "properties": {
      "name": "[variables('app_webjobs_name')]",
      "hostNames": [
        "[concat(variables('app_webjobs_name'),'.azurewebsites.net')]"
      ],
      "enabledHostNames": [
        "[concat(variables('app_webjobs_name'),'.azurewebsites.net')]",
        "[concat(variables('app_webjobs_name'),'.scm.azurewebsites.net')]"
      ],
      "hostNameSslStates": [
        {
          "name": "[concat(variables('app_webjobs_name'),variables('app_webjobs_name'),'.azurewebsites.net')]",
          "sslState": 0,
          "ipBasedSslState": 0
        },
        {
          "name": "[concat(variables('app_webjobs_name'),variables('app_webjobs_name'),'.scm.azurewebsites.net')]",
          "sslState": 0,
          "ipBasedSslState": 0
        }
      ],
      "serverFarmId": "[resourceId('Microsoft.Web/serverfarms', parameters('serverfarm_app_plan_name'))]",
      "siteConfig": {
        "AlwaysOn": true
      }
    },
    "resources": [
      {
        "name": "appsettings",
        "type": "config",
        "apiVersion": "2015-08-01",
        "location": "[parameters('datacenter_region')]",
        "tags": {
          "displayName": "WebAppSettings"
        },
        "properties": {
          "CognetiveServicesAccountKey": "[listKeys(resourceId('Microsoft.CognitiveServices/accounts', parameters('cognetive_services_name')), '2016-02-01-preview').key1]"
        },
        "dependsOn": [
          "[concat('Microsoft.Web/sites/',variables('app_webjobs_name'))]"
        ]
      },
      {
        "name": "connectionstrings",
        "type": "config",
        "apiVersion": "2015-08-01",
        "location": "[parameters('datacenter_region')]",
        "properties": {
          "FeedbackDatabaseConnection": {
            "value": "[variables('database_connection_string')]",
            "type": "SQLAzure"
          },
          "AzureWebJobsDashboard": {
            "value": "[concat('DefaultEndpointsProtocol=https;AccountName=',variables('storageaccount_name'),';AccountKey=',listKeys(resourceId('Microsoft.Storage/storageAccounts',variables('storageaccount_name')),'2016-01-01').keys[0].value)]",
            "type": "Custom"
          },
          "AzureWebJobsStorage": {
            "value": "[concat('DefaultEndpointsProtocol=https;AccountName=',variables('storageaccount_name'),';AccountKey=',listKeys(resourceId('Microsoft.Storage/storageAccounts',variables('storageaccount_name')),'2016-01-01').keys[0].value)]",
            "type": "Custom"
          }
        },
        "dependsOn": [
          "[concat('Microsoft.Web/sites/',variables('app_webjobs_name'))]"
        ]
      }
    ],
    "dependsOn": [
      "[resourceId('Microsoft.Web/serverfarms',parameters('serverfarm_app_plan_name'))]"
    ]
  },
  {
    "type": "Microsoft.Storage/storageAccounts",
    "sku": "[parameters('storageaccount_sku')]",
    "kind": "Storage",
    "name": "[variables('storageaccount_name')]",
    "apiVersion": "2016-01-01",
    "location": "[parameters('datacenter_region')]",
    "tags": {},
    "properties": {},
    "resources": [],
    "dependsOn": []
  },
  {
    "name": "[parameters('cognetive_services_name')]",
    "type": "Microsoft.CognitiveServices/accounts",
    "apiVersion": "2016-02-01-preview",
    "sku": {
      "name": "[parameters('cognetive_services_sku')]"
    },
    "kind": "TextAnalytics",
    "location": "[parameters('datacenter_region')]",
    "tags": {},
    "properties": {}
  }
```
*NOTE:* Of particular importance here are the **appsettings** and **connectionstrings** of the new Web Job site. The values of these configurations, such as **CognetiveServicesAccountKey**, are queried out of the resources *as they are being provisioned*. Without this powerful capability of ARM we would have to provision all the resources and then separately manually configure these settings in the Azure Portal, or (much worse) in our source code.
1. Open "prod.json" and add the following parameters JSON:
```
    "app_webjobs_name_prefix": {
      "value": "gab2017-webjobs"
    },
    "storageaccount_name_prefix": {
      "value": "gab2017"
    },
    "cognetive_services_name": {
      "value": "gab2017-textanalytics"
    }
```
1. Open "dev.json" and add the following parameters JSON:
```
    "app_webjobs_name_prefix": {
      "value": "gab2017-webjobs-dev"
    },
    "storageaccount_name_prefix": {
      "value": "gab2017dev"
    },
    "cognetive_services_name": {
      "value": "gab2017-textanalytics-dev"
    },
    "cognetive_services_sku": {
      "value": "F0"
    }
```
*NOTE:* For dev, we opt for using the free-tier of Cognitive services. This shows that if you are strategic in choosing available parameter inputs, you can have a high degree of control between environments and even deployments within the same environment by varying the parameter values. These configurations are what the Web Job to be added in the next section use to gain access to the database, Azure Storage Account, and Cognitive Services.

### 4.c Adding the Web Job Project

Adding the Web Job to the solution involves 3 things: provisioning the deploy target resource (will be done as a result of the previous step), configuring the Delivery Pipeline to deploy the job, and adding the Web Job to the Visual Studio solution to be included in the CI build.

1. Go to VSTS and again edit the release definition
1. Add another task **Azure PowerShell** and configure the task as follows:
   * Change **Azure Connection Type** to **Azure Resource Manager**
   * Select the subscription you linked to VSTS
   * Change **Script Type** to **Inline Script**
   * Add the following **Inline Script** text:
    ```
    $resourceGroup = 'GAB2017Dev'
    $apps = Get-AzureRmWebApp -ResourceGroupName $resourceGroup
    foreach ($app in $apps)
    {
      $appName = $app.Name
      if ($appName.StartsWith('gab2017-webjobs-'))
      {
        Write-Host ("Found app service: $appName")
        Write-Output ("##vso[task.setvariable variable=webJobServiceName;]$appName")
      }
    }
    ```
1. Add another task **Azure App Service Deploy** and configure the task as follows:
   * Select the subscription you linked to VSTS
   * Enter `$(webJobServiceName)` as the **App service name** (this is a different variable captured by the second PowerShell script)
   * Update the **Package or folder** to `$(System.DefaultWorkingDirectory)/GAB2017 CI/drop/Feedback.SentimentCalculator.zip`
   <br />*NOTE:* As with the website, there was a bit of setup I took care of for you involving adding Web Job SDK NuGet packages and adding a publish profile.
1. Repeat the above 2 steps for the **Prod** environment, except adjust the first line of the **Inline Script** to assign resource group name `GAB2017`
1. Save the release definition
1. Go to Visual Studio
1. Right click the Feedback solution within Solution Explorer and click *Add* > *Existing Project* from the context menu
1. Navigate to the **Feedback.SentimentCalculator\Feedback.SentimentCalculator.csproj** project within the source repository and click **Open**
<br />*NOTE:* The Web Job is fully implemented to process the submitted feedback using Cognitive Services.
1. Commit the sources to trigger the build and automatic deployment to the newly-created development environment using these commands:
```
git add .
git commit -m "Added web job and updated ARM template to provision new resources"
git push
```

### 4.d Promoting to Production

You can check the progress of the running build process by going to the Build section of VSTS. When the build completes a release to the Dev environment should automatically pick up. When done, your **Releases** list should look something like this:
<br />![Released to Dev, Prod pending](images/released-to-dev.png)

If you go to the deployed sites in Dev, you should notice that after submitting feedback, within a minute of refreshing the site the feedback has a sentiment score value. This behavior doesn't yet exist in Prod.
<br />*TIP:* If you don't see this behavior, you can check KUDU's Web Job logs at https://gab2017-webjobs-{random-value}.scm.azurewebsites.net/azurejobs. You will need to log in with your Azure credentials. That *.scm* endpoint has tons of Ops tools for supporting your Azure App Service deployments.

Furthermore, if you log in to the Azure Portal you'll notice the GAB2017Dev resource group has several more resources provisioned than GAB2017. Remember and be amazed that you never manually created any of these resources!

Now you can promote the latest to production simply by double-clicking on the VSTS release, clicking the Deploy button, selecting the target environment, and confirming the deployment.
<br />![Promoting to prod](images/promote-to-prod.png)

After that completes Dev and Prod should behave the same and the Azure Portal should show both resources groups having the same type and number of resources.
