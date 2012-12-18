#include <sys/wait.h>
#include <stdlib.h>
#include <stdio.h>
#include <unistd.h>

int main(void)
{
        pid_t pids[10];
        int i;

        for (i = 9; i >= 0; --i) {
                pids[i] = fork();
                if (pids[i] == 0) {
                        printf("hello %i\n",i);
                        sleep(i+1);
                        _exit(0);
                }
                printf("child %d\n",pids[i]);
        }

        for (i = 9; i >= 0; --i) {
                printf("wait  %d\n",pids[i]);
                waitpid(pids[i], NULL, 0);
         }

        return 0;
}
