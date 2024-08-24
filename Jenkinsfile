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
        
        stage ("Remove all containers and images"){
             steps{
               sh'''#!/bin/sh 
            sudo /home/azureuser/delete.sh
 '''
            }
          }
        stage ("Remove docker cache"){
         steps{
            sh "sudo docker system prune -af"
         }   
         }   
}
