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
                sh "find FrontEnd/my-app/ -type f -exec sed  -i 's#http://localhost:5034#https://10.20.34.102/api#g' {} +"
             }
         }
         stage("Change IP in appsettings.json")
         {
             steps{
                sh "find BackEnd/Amazon-clone/ -type f -exec sed  -i 's#http://localhost:81#https://10.20.34.102/#g' {} +"
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
    }
}
