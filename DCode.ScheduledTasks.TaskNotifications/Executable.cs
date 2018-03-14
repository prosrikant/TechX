﻿using DCode.ScheduledTasks.Operations.TaskNotifications;
using DCode.ScheduledTasks.TaskNotifications.Operations;
using Ninject;
using System;

namespace DCode.ScheduledTasks.TaskNotifications
{
    public class Executable
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Entered App");

                var kernel = new StandardKernel(new NotificationManagerModule());

                Console.WriteLine("Initialized Kernel");

                var taskNotificationOperation = new TaskNotificationsOperation(kernel);

                var firmInitiativeNotificationOperation = new FirmInitiativeNotificationOperation(kernel);

                Console.WriteLine("Initialized operation");

                taskNotificationOperation.Invoke();

                Console.WriteLine("Done sending out newly added task notifications");

                firmInitiativeNotificationOperation.Invoke();

                Console.WriteLine("Done sending out firm initiative notifications to registered users");

                Console.WriteLine("Operation ended");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error::" + ex.StackTrace);
            }

            Console.ReadKey();
        }
    }
}
