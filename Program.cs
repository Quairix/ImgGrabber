using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace LoadSite
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Let's start...");
            var driver = new ChromeDriver(@"C:\ForGit\UE4First\ImgGrabber\ImgGrabber");//Директория с ChromeDriver
            driver.Url = "https://elib.bstu.ru/Account/OpenID";
            driver.FindElement(By.Id("ya")).Click();


            driver.FindElement(By.XPath("//input[@type='submit']")).Click();

            var inputEmail = driver.FindElement(By.XPath("//input[@name='login']"));//это Id инпута для ввода email
            Console.WriteLine("Email: ");
            inputEmail.SendKeys(Console.ReadLine());

            var Pass = driver.FindElement(By.XPath("//input[@name='passwd']"));//Ввод пароля
            Console.WriteLine("Password: ");
            Pass.SendKeys(Console.ReadLine());

            driver.FindElement(By.ClassName("passport-Button-Text")).Click();
            
            //if (driver.Title != "Поиск книг и просмотр каталога")
            //{
            //    throw new System.ArgumentException("Wrong title");
            //}

            Console.WriteLine("Success");
            WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 5));
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("BasicSearchString")));
            driver.Navigate().GoToUrl("https://elib.bstu.ru/Reader/Book/2015122912023510900000657043");
            //driver.Navigate().GoToUrl("https://elib.bstu.ru/?searchType=User&BasicSearchString=%D0%B3%D0%BB%D1%83%D1%85%D0%BE%D0%B5%D0%B4%D0%BE%D0%B2&ViewMode=false&PackId=0&page=1");
            int i = new int();
            i = 1;
            string pagenumber = "1";

            while (pagenumber==i.ToString())//160 pages
            {
                var base64string = driver.ExecuteScript(@"
                    var c = document.createElement('canvas');
                    var ctx = c.getContext('2d');
                    var img = document.getElementsByTagName('img')[1];
                    c.height=img.height;
                    c.width=img.width;
                    ctx.drawImage(img, 0, 0,img.width, img.height);
                    var base64String = c.toDataURL();
                    return base64String;
                    ") as string;

                var base64 = base64string.Split(',').Last();
                using (var stream = new MemoryStream(Convert.FromBase64String(base64)))
                {
                    using (var bitmap = new Bitmap(stream))
                    {
                        var filepath = Path.Combine(@"C:\book\image", $"image{i}.jpg");
                        bitmap.Save(filepath, ImageFormat.Jpeg);
                    }
                }

                driver.FindElement(By.XPath("//input[@class='isbutton nextbutton']")).Click();
                pagenumber = driver.FindElement(By.XPath("//input[@name='pagenum']")).GetAttribute("Value");
                i++;
                Thread.Sleep(3000);
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("thePage")));
            }
            //driver.FindElement(By.XPath("//a[@onclick='CheckOpenBook(\'2015122912023510900000657043\', false); return false;']")).Click();
        }
    }
}