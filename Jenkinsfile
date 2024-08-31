#!groovy
//  groovy Jenkinsfile
pipeline  {
    agent any;
    stages {
        
         stage("Backup files")
         {
             steps{
                sh """
                #!/bin/bash
                hostname
                """
             }
         }
        stage("Change IP in axios.js")
         {
             steps{
                sh "find FrontEnd/my-app/ -type f -exec sed  -i 's#http://localhost:5034#https://10.20.34.232/api#g' {} +"
             }
         }
         stage("Change IP in appsettings.json")
         {
             steps{
                sh "find BackEnd/Amazon-clone/ -type f -exec sed  -i 's#http://localhost:81#https://10.20.34.232/#g' {} +"
             }
         }
         stage ("Remove all containers and images"){
             steps{
               sh'''#!/bin/sh 
            bash delete.sh -y
 '''
             }
        }
         stage ("Remove docker cache"){
         steps{
            sh "docker system prune -af"
         }   
        }
         stage ("Run MSSQL container"){
            steps{
                sh 'docker run  --restart=always -v /home/db:/var/opt/mssql -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Qwerty-1" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest'
            }
        }
        stage("Create frontend docker image") {
            steps {
                echo 'Creating frontend docker image ...'
                sh "cd FrontEnd/my-app && docker build --no-cache -t levchuk19/amazon-clone-frontend   . "                
            }
        }
         stage("Create backend docker image") {
            steps {
                echo 'Creating backend docker image ...'
                sh " cd Amazon-Clone/BackEnd/Amazon-clone/ && docker build --no-cache -t levchuk19/amazon-clone-backend  . "
            }
        }
         stage("docker frontend run") {
             steps {
                 echo " ============== Creating frontend docker container =================="
                 sh '''
                 docker run -d --restart=always -p 80:80 --name=frontend levchuk19/amazon-clone-frontend
                 '''
             }
         }
        stage("docker backend run") {
             steps {
                 echo " ============== Creating backend docker container =================="
                 sh '''
                 docker run -d --restart=always -p 5034:5034 --name=backend levchuk19/amazon-clone-backend
                 '''
             }
        }
    }
}
