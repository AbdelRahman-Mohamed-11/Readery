
using Microsoft.AspNetCore.Identity;
using Readery.Core.Models;
using Readery.Core.Models.Identity;
using Readery.Web.Models;

namespace Readery.DataAccess.Data.Seeder;

public class SeedData(RoleManager<ApplicationRole> roleManager)
{
    public async static Task SeedAsync(ApplicationDbContext dbContext)
    {
        if (!dbContext.Categories.Any())
        {
            dbContext.Categories.AddRange(
                new() { Name = "Action", DisplayOrder = 0 },
                new() { Name = "Scifi", DisplayOrder = 1 },
                new() { Name = "History", DisplayOrder = 2 }
            );

            await dbContext.SaveChangesAsync();
        }

        if (!dbContext.Products.Any())
        {
            dbContext.Products.AddRange(
                new()
                {

                    Title = "The Great Gatsby",
                    Author = "F. Scott Fitzgerald",
                    Description = "A classic American novel that captures the essence of the Jazz Age and explores themes of wealth, love, and the American Dream.",
                    ISBN = "GAT0010001",
                    ListPrice = 99,
                    Price = 90,
                    Price50 = 85,
                    Price100 = 80,
                    CategoryId = 1,
                },

                new()
                {

                    Title = "To Kill a Mockingbird",
                    Author = "Harper Lee",
                    Description = "Set in the Deep South, this novel explores racial injustice and moral growth through the eyes of a young girl, Scout Finch.",
                    ISBN = "TKM0020001",
                    ListPrice = 40,
                    Price = 30,
                    Price50 = 25,
                    Price100 = 20,
                    CategoryId = 1,

                },
                new()
                {
                    Title = "Don Quixote",
                    Author = "Miguel de Cervantes",
                    Description = "The story of a nobleman who, after reading too many chivalric romances, sets out to revive chivalry in Spain.",
                    ISBN = "DQ046001",
                    ListPrice = 85,
                    Price = 80,
                    Price50 = 75,
                    Price100 = 70,
                    CategoryId = 1,
                    ImageUrl = ""
                },
    new()
    {
        Title = "Crime and Punishment",
        Author = "Fyodor Dostoevsky",
        Description = "A psychological exploration of morality, guilt, and redemption following the story of a young man who commits a murder.",
        ISBN = "CAP047001",
        ListPrice = 65,
        Price = 60,
        Price50 = 55,
        Price100 = 50,
        CategoryId = 1,
        ImageUrl = ""
    },
    new()
    {
        Title = "War and Peace",
        Author = "Leo Tolstoy",
        Description = "A monumental work that intertwines the lives of five families as Russia faces Napoleon's invasion.",
        ISBN = "WAP048001",
        ListPrice = 90,
        Price = 85,
        Price50 = 80,
        Price100 = 75,
        CategoryId = 2,
        ImageUrl = ""
    },
    new()
    {
        Title = "Anna Karenina",
        Author = "Leo Tolstoy",
        Description = "A tragic love story that explores the life of a married aristocrat and her affair with the affluent Count Vronsky.",
        ISBN = "AK049001",
        ListPrice = 70,
        Price = 65,
        Price50 = 60,
        Price100 = 55,
        CategoryId = 2,
        ImageUrl = ""
    },
    new()
    {
        Title = "Brave New World",
        Author = "Aldous Huxley",
        Description = "A dystopian novel set in a future society that controls individuals through technology and conditioning.",
        ISBN = "BNW050001",
        ListPrice = 55,
        Price = 50,
        Price50 = 45,
        Price100 = 40,
        CategoryId = 3,
        ImageUrl = ""
    },
    new()
    {
        Title = "The Brothers Karamazov",
        Author = "Fyodor Dostoevsky",
        Description = "A philosophical novel that delves into deep moral, ethical, and religious questions within a family saga.",
        ISBN = "TBK051001",
        ListPrice = 75,
        Price = 70,
        Price50 = 65,
        Price100 = 60,
        CategoryId = 3,
        ImageUrl = ""
    },
    new()
    {
        Title = "The Odyssey",
        Author = "Homer",
        Description = "An epic poem about the adventures of Odysseus as he tries to return home after the Trojan War.",
        ISBN = "ODYS052001",
        ListPrice = 50,
        Price = 45,
        Price50 = 40,
        Price100 = 35,
        CategoryId = 1,
        ImageUrl = ""
    },
    new()
    {
        Title = "The Iliad",
        Author = "Homer",
        Description = "An epic poem recounting the events of the Trojan War, focusing on the hero Achilles.",
        ISBN = "ILI053001",
        ListPrice = 50,
        Price = 45,
        Price50 = 40,
        Price100 = 35,
        CategoryId = 1,
        ImageUrl = ""
    },
    new()
    {
        Title = "The Divine Comedy",
        Author = "Dante Alighieri",
        Description = "A three-part epic poem that takes the reader through Hell, Purgatory, and Paradise.",
        ISBN = "DIV054001",
        ListPrice = 80,
        Price = 75,
        Price50 = 70,
        Price100 = 65,
        CategoryId = 2,
        ImageUrl = ""
    },
    new()
    {
        Title = "Ulysses",
        Author = "James Joyce",
        Description = "A modernist novel that chronicles the life of Leopold Bloom during a single day in Dublin.",
        ISBN = "ULY055001",
        ListPrice = 85,
        Price = 80,
        Price50 = 75,
        Price100 = 70,
        CategoryId = 2,
        ImageUrl = ""
    },
    new()
    {
        Title = "The Sound and the Fury",
        Author = "William Faulkner",
        Description = "A Southern Gothic novel that tells the decline of the Compson family from four different perspectives.",
        ISBN = "SAF056001",
        ListPrice = 70,
        Price = 65,
        Price50 = 60,
        Price100 = 55,
        CategoryId = 3,
        ImageUrl = ""
    },
    new()
    {
        Title = "Wuthering Heights",
        Author = "Emily Brontë",
        Description = "A gothic novel that explores the intense and destructive relationship between Catherine Earnshaw and Heathcliff.",
        ISBN = "WH057001",
        ListPrice = 55,
        Price = 50,
        Price50 = 45,
        Price100 = 40,
        CategoryId = 3,
        ImageUrl = ""
    },
    new()
    {
        Title = "The Picture of Dorian Gray",
        Author = "Oscar Wilde",
        Description = "A novel about a man who remains young while his portrait ages, reflecting the corruption of his soul.",
        ISBN = "PDG058001",
        ListPrice = 45,
        Price = 40,
        Price50 = 35,
        Price100 = 30,
        CategoryId = 1,
        ImageUrl = ""
    },
    new()
    {
        Title = "Les Misérables",
        Author = "Victor Hugo",
        Description = "An epic novel set in post-revolutionary France, exploring themes of justice, love, and redemption.",
        ISBN = "LES059001",
        ListPrice = 95,
        Price = 90,
        Price50 = 85,
        Price100 = 80,
        CategoryId = 1,
        ImageUrl = ""
    },
    new()
    {
        Title = "Frankenstein",
        Author = "Mary Shelley",
        Description = "A gothic novel that tells the story of Victor Frankenstein, a scientist who creates a sentient creature.",
        ISBN = "FRK060001",
        ListPrice = 50,
        Price = 45,
        Price50 = 40,
        Price100 = 35,
        CategoryId = 2,
        ImageUrl = ""
    },
    new()
    {
        Title = "The Hunchback of Notre-Dame",
        Author = "Victor Hugo",
        Description = "A tragic novel about the deformed bell-ringer Quasimodo and his unrequited love for Esmeralda.",
        ISBN = "HND061001",
        ListPrice = 85,
        Price = 80,
        Price50 = 75,
        Price100 = 70,
        CategoryId = 2,
        ImageUrl = ""
    },
    new()
    {
        Title = "A Christmas Carol",
        Author = "Charles Dickens",
        Description = "A novella about the miserly Ebenezer Scrooge's redemption after being visited by three ghosts on Christmas Eve.",
        ISBN = "XMAS062001",
        ListPrice = 35,
        Price = 30,
        Price50 = 25,
        Price100 = 20,
        CategoryId = 3,
        ImageUrl = ""
    },
    new()
    {
        Title = "Dracula",
        Author = "Bram Stoker",
        Description = "A gothic horror novel that tells the story of Count Dracula's attempt to move from Transylvania to England.",
        ISBN = "DRAC063001",
        ListPrice = 50,
        Price = 45,
        Price50 = 40,
        Price100 = 35,
        CategoryId = 3,
        ImageUrl = ""
    },
    new()
    {
        Title = "The Three Musketeers",
        Author = "Alexandre Dumas",
        Description = "A historical novel set in 17th century France, following the adventures of d'Artagnan and his friends Athos, Porthos, and Aramis.",
        ISBN = "MUSK064001",
        ListPrice = 75,
        Price = 70,
        Price50 = 65,
        Price100 = 60,
        CategoryId = 1,
        ImageUrl = ""
    },
    new()
    {
        Title = "The Count of Monte Cristo",
        Author = "Alexandre Dumas",
        Description = "A story of betrayal, imprisonment, and revenge set in post-Napoleonic France.",
        ISBN = "CMC065001",
        ListPrice = 95,
        Price = 90,
        Price50 = 85,
        Price100 = 80,
        CategoryId = 1,
        ImageUrl = ""
    },
                new()
                {

                    Title = "Moby-Dick",
                    Author = "Herman Melville",
                    Description = "An epic tale of obsession and revenge as Captain Ahab hunts the elusive white whale, Moby-Dick, across the seas.",
                    ISBN = "MDK0030001",
                    ListPrice = 55,
                    Price = 50,
                    Price50 = 40,
                    Price100 = 35,
                    CategoryId = 2,

                },

                new()
                {

                    Title = "The Catcher in the Rye",
                    Author = "J.D. Salinger",
                    Description = "This coming-of-age story follows Holden Caulfield, a disillusioned teenager, as he navigates the complexities of adolescence.",
                    ISBN = "CITR0040001",
                    ListPrice = 70,
                    Price = 65,
                    Price50 = 60,
                    Price100 = 55,
                    CategoryId = 2,

                },

                new()
                {

                    Title = "The Grapes of Wrath",
                    Author = "John Steinbeck",
                    Description = "A powerful novel that chronicles the hardships faced by migrant farmers during the Great Depression in America.",
                    ISBN = "GOW0050001",
                    ListPrice = 30,
                    Price = 27,
                    Price50 = 25,
                    Price100 = 20,
                    CategoryId = 2,

                },

                new()
                {

                    Title = "The Road",
                    Author = "Cormac McCarthy",
                    Description = "A haunting story of survival in a post-apocalyptic world, as a father and son journey through a desolate landscape.",
                    ISBN = "TRD0060001",
                    ListPrice = 25,
                    Price = 23,
                    Price50 = 22,
                    Price100 = 20,
                    CategoryId = 3,

                },
                new()
                {
                    Title = "1984",
                    Author = "George Orwell",
                    Description = "A dystopian novel set in a totalitarian society controlled by Big Brother.",
                    ISBN = "1984007001",
                    ListPrice = 50,
                    Price = 45,
                    Price50 = 40,
                    Price100 = 35,
                    CategoryId = 1,
                    ImageUrl = ""
                },
    new()
    {
        Title = "Brave New World",
        Author = "Aldous Huxley",
        Description = "A futuristic society driven by technological control and consumption.",
        ISBN = "BNW007001",
        ListPrice = 60,
        Price = 55,
        Price50 = 50,
        Price100 = 45,
        CategoryId = 1,
        ImageUrl = ""
    },
    new()
    {
        Title = "Pride and Prejudice",
        Author = "Jane Austen",
        Description = "A romantic novel about manners and marriage in early 19th-century England.",
        ISBN = "PAP008001",
        ListPrice = 40,
        Price = 35,
        Price50 = 30,
        Price100 = 25,
        CategoryId = 2,
        ImageUrl = ""
    },
    new()
    {
        Title = "The Hobbit",
        Author = "J.R.R. Tolkien",
        Description = "A fantasy novel about Bilbo Baggins' adventure to win a share of treasure.",
        ISBN = "HOB009001",
        ListPrice = 80,
        Price = 75,
        Price50 = 70,
        Price100 = 65,
        CategoryId = 2,
        ImageUrl = ""
    },
    new()
    {
        Title = "War and Peace",
        Author = "Leo Tolstoy",
        Description = "A historical novel set during the Napoleonic Wars in Russia.",
        ISBN = "WAP010001",
        ListPrice = 100,
        Price = 90,
        Price50 = 80,
        Price100 = 70,
        CategoryId = 3,
        ImageUrl = ""
    },
    new()
    {
        Title = "Jane Eyre",
        Author = "Charlotte Brontë",
        Description = "A coming-of-age story that tackles issues of class, sexuality, and religion.",
        ISBN = "JEY011001",
        ListPrice = 45,
        Price = 40,
        Price50 = 35,
        Price100 = 30,
        CategoryId = 3,
        ImageUrl = ""
    },
    new()
    {
        Title = "Crime and Punishment",
        Author = "Fyodor Dostoevsky",
        Description = "A psychological novel about the moral dilemmas of a young intellectual.",
        ISBN = "CAP012001",
        ListPrice = 55,
        Price = 50,
        Price50 = 45,
        Price100 = 40,
        CategoryId = 1,
        ImageUrl = ""
    },
    new()
    {
        Title = "Frankenstein",
        Author = "Mary Shelley",
        Description = "A gothic novel about the consequences of playing God.",
        ISBN = "FRK013001",
        ListPrice = 35,
        Price = 30,
        Price50 = 25,
        Price100 = 20,
        CategoryId = 2,
        ImageUrl = ""
    },
    new()
    {
        Title = "Dracula",
        Author = "Bram Stoker",
        Description = "A classic gothic horror novel that tells the story of Count Dracula.",
        ISBN = "DRC014001",
        ListPrice = 60,
        Price = 55,
        Price50 = 50,
        Price100 = 45,
        CategoryId = 2,
        ImageUrl = ""
    },
    new()
    {
        Title = "The Odyssey",
        Author = "Homer",
        Description = "An ancient Greek epic poem about the hero Odysseus' journey home after the Trojan War.",
        ISBN = "ODY015001",
        ListPrice = 100,
        Price = 90,
        Price50 = 80,
        Price100 = 70,
        CategoryId = 3,
        ImageUrl = ""
    },
    // Add 45 more products below similarly
    new()
    {
        Title = "Don Quixote",
        Author = "Miguel de Cervantes",
        Description = "A comedic novel that follows the adventures of a man who believes he is a knight.",
        ISBN = "DQX016001",
        ListPrice = 75,
        Price = 70,
        Price50 = 65,
        Price100 = 60,
        CategoryId = 3,
        ImageUrl = ""
    },
    new()
    {
        Title = "The Divine Comedy",
        Author = "Dante Alighieri",
        Description = "An epic poem that describes Dante's journey through Hell, Purgatory, and Heaven.",
        ISBN = "DDC017001",
        ListPrice = 120,
        Price = 110,
        Price50 = 100,
        Price100 = 90,
        CategoryId = 1,
        ImageUrl = ""
    },
    new()
    {
        Title = "The Iliad",
        Author = "Homer",
        Description = "An epic poem about the Trojan War and the hero Achilles.",
        ISBN = "ILI018001",
        ListPrice = 110,
        Price = 100,
        Price50 = 90,
        Price100 = 80,
        CategoryId = 1,
        ImageUrl = ""
    },
    new()
    {
        Title = "The Picture of Dorian Gray",
        Author = "Oscar Wilde",
        Description = "A philosophical novel about vanity and moral corruption.",
        ISBN = "POG019001",
        ListPrice = 50,
        Price = 45,
        Price50 = 40,
        Price100 = 35,
        CategoryId = 2,
        ImageUrl = ""
    },
    new()
    {
        Title = "Les Misérables",
        Author = "Victor Hugo",
        Description = "A French novel about redemption and the struggle for justice.",
        ISBN = "LM020001",
        ListPrice = 150,
        Price = 140,
        Price50 = 130,
        Price100 = 120,
        CategoryId = 3,
        ImageUrl = ""
    },
    new()
    {
        Title = "Anna Karenina",
        Author = "Leo Tolstoy",
        Description = "A novel about love, infidelity, and societal norms in 19th-century Russia.",
        ISBN = "AK021001",
        ListPrice = 95,
        Price = 85,
        Price50 = 75,
        Price100 = 65,
        CategoryId = 1,
        ImageUrl = ""
    },
    new()
    {
        Title = "The Brothers Karamazov",
        Author = "Fyodor Dostoevsky",
        Description = "A philosophical novel exploring faith, free will, and morality.",
        ISBN = "TBK022001",
        ListPrice = 85,
        Price = 75,
        Price50 = 70,
        Price100 = 60,
        CategoryId = 1,
        ImageUrl = ""
    },
    new()
    {
        Title = "The Count of Monte Cristo",
        Author = "Alexandre Dumas",
        Description = "A story of revenge, justice, and redemption set in post-Napoleonic France.",
        ISBN = "CMC023001",
        ListPrice = 130,
        Price = 120,
        Price50 = 110,
        Price100 = 100,
        CategoryId = 2,
        ImageUrl = ""
    },
    new()
    {
        Title = "The Three Musketeers",
        Author = "Alexandre Dumas",
        Description = "An adventure novel about the friendship and heroism of three musketeers.",
        ISBN = "T3M024001",
        ListPrice = 90,
        Price = 80,
        Price50 = 70,
        Price100 = 60,
        CategoryId = 2,
        ImageUrl = ""
    },
    new()
    {
        Title = "The Hunchback of Notre-Dame",
        Author = "Victor Hugo",
        Description = "A tragic novel about love and social justice set in medieval Paris.",
        ISBN = "HND025001",
        ListPrice = 100,
        Price = 90,
        Price50 = 80,
        Price100 = 70,
        CategoryId = 3,
        ImageUrl = ""
    }
    , new()
    {
        Title = "Fahrenheit 451",
        Author = "Ray Bradbury",
        Description = "A dystopian novel about a future where books are banned and 'firemen' burn any that are found.",
        ISBN = "F451026001",
        ListPrice = 45,
        Price = 40,
        Price50 = 35,
        Price100 = 30,
        CategoryId = 1,
        ImageUrl = ""
    },
    new()
    {
        Title = "The Sun Also Rises",
        Author = "Ernest Hemingway",
        Description = "A novel about a group of expatriates traveling from Paris to Spain and their individual struggles.",
        ISBN = "SAR027001",
        ListPrice = 65,
        Price = 60,
        Price50 = 55,
        Price100 = 50,
        CategoryId = 1,
        ImageUrl = ""
    },
    new()
    {
        Title = "Lord of the Flies",
        Author = "William Golding",
        Description = "A group of boys stranded on an uninhabited island must govern themselves, leading to chaos.",
        ISBN = "LOF028001",
        ListPrice = 40,
        Price = 35,
        Price50 = 30,
        Price100 = 25,
        CategoryId = 2,
        ImageUrl = ""
    },
    new()
    {
        Title = "The Stranger",
        Author = "Albert Camus",
        Description = "A novel that explores existentialism through the story of Meursault, an indifferent man who commits murder.",
        ISBN = "STR029001",
        ListPrice = 55,
        Price = 50,
        Price50 = 45,
        Price100 = 40,
        CategoryId = 2,
        ImageUrl = ""
    },
    new()
    {
        Title = "Slaughterhouse-Five",
        Author = "Kurt Vonnegut",
        Description = "A science fiction-infused anti-war novel based on Vonnegut's experiences during World War II.",
        ISBN = "SHV030001",
        ListPrice = 60,
        Price = 55,
        Price50 = 50,
        Price100 = 45,
        CategoryId = 3,
        ImageUrl = ""
    },
    new()
    {
        Title = "Catch-22",
        Author = "Joseph Heller",
        Description = "A satirical novel set during World War II about the absurdity of war and bureaucracy.",
        ISBN = "C22_031001",
        ListPrice = 75,
        Price = 70,
        Price50 = 65,
        Price100 = 60,
        CategoryId = 3,
        ImageUrl = ""
    },
    new()
    {
        Title = "Of Mice and Men",
        Author = "John Steinbeck",
        Description = "A novella about two displaced ranch workers during the Great Depression in California.",
        ISBN = "OMM032001",
        ListPrice = 35,
        Price = 30,
        Price50 = 25,
        Price100 = 20,
        CategoryId = 1,
        ImageUrl = ""
    },
    new()
    {
        Title = "Gone with the Wind",
        Author = "Margaret Mitchell",
        Description = "An American classic about love and survival in the South during the Civil War.",
        ISBN = "GWW033001",
        ListPrice = 80,
        Price = 75,
        Price50 = 70,
        Price100 = 65,
        CategoryId = 1,
        ImageUrl = ""
    },
    new()
    {
        Title = "The Call of the Wild",
        Author = "Jack London",
        Description = "A story about a domesticated dog's survival instincts taking over after being thrust into the Alaskan wilderness.",
        ISBN = "COW034001",
        ListPrice = 45,
        Price = 40,
        Price50 = 35,
        Price100 = 30,
        CategoryId = 2,
        ImageUrl = ""
    },
    new()
    {
        Title = "The Catch-22",
        Author = "Joseph Heller",
        Description = "A satirical novel about the illogical nature of war and military bureaucracy.",
        ISBN = "CTT035001",
        ListPrice = 60,
        Price = 55,
        Price50 = 50,
        Price100 = 45,
        CategoryId = 2,
        ImageUrl = ""
    },
    new()
    {
        Title = "East of Eden",
        Author = "John Steinbeck",
        Description = "A novel that explores the nature of good and evil, as two families intertwine in California's Salinas Valley.",
        ISBN = "EOE036001",
        ListPrice = 70,
        Price = 65,
        Price50 = 60,
        Price100 = 55,
        CategoryId = 3,
        ImageUrl = ""
    },
    new()
    {
        Title = "The Old Man and the Sea",
        Author = "Ernest Hemingway",
        Description = "A short novel about an old fisherman's struggle with a giant marlin.",
        ISBN = "OMS037001",
        ListPrice = 35,
        Price = 30,
        Price50 = 25,
        Price100 = 20,
        CategoryId = 3,
        ImageUrl = ""
    },
    new()
    {
        Title = "The Scarlet Letter",
        Author = "Nathaniel Hawthorne",
        Description = "A historical novel about the consequences of sin, guilt, and legalism in Puritan Massachusetts.",
        ISBN = "TSL038001",
        ListPrice = 55,
        Price = 50,
        Price50 = 45,
        Price100 = 40,
        CategoryId = 1,
        ImageUrl = ""
    },
    new()
    {
        Title = "The Jungle",
        Author = "Upton Sinclair",
        Description = "A muckraking novel about the harsh conditions and exploited lives of immigrants in the United States.",
        ISBN = "TJ039001",
        ListPrice = 65,
        Price = 60,
        Price50 = 55,
        Price100 = 50,
        CategoryId = 1,
        ImageUrl = ""
    },
    new()
    {
        Title = "The Bell Jar",
        Author = "Sylvia Plath",
        Description = "A semi-autobiographical novel about a young woman's struggle with depression.",
        ISBN = "TBJ040001",
        ListPrice = 50,
        Price = 45,
        Price50 = 40,
        Price100 = 35,
        CategoryId = 2,
        ImageUrl = ""
    },
    new()
    {
        Title = "White Fang",
        Author = "Jack London",
        Description = "A companion novel to The Call of the Wild, this tells the story of a wolf-dog's domestication.",
        ISBN = "WF041001",
        ListPrice = 40,
        Price = 35,
        Price50 = 30,
        Price100 = 25,
        CategoryId = 2,
        ImageUrl = ""
    },
    new()
    {
        Title = "Heart of Darkness",
        Author = "Joseph Conrad",
        Description = "A novella about a voyage up the Congo River and the nature of imperialism.",
        ISBN = "HOD042001",
        ListPrice = 45,
        Price = 40,
        Price50 = 35,
        Price100 = 30,
        CategoryId = 3,
        ImageUrl = ""
    },
    new()
    {
        Title = "The Metamorphosis",
        Author = "Franz Kafka",
        Description = "A novella about a man who wakes up one morning transformed into a giant insect.",
        ISBN = "TMT043001",
        ListPrice = 30,
        Price = 27,
        Price50 = 25,
        Price100 = 22,
        CategoryId = 3,
        ImageUrl = ""
    },
    new()
    {
        Title = "A Tale of Two Cities",
        Author = "Charles Dickens",
        Description = "A historical novel set in London and Paris during the French Revolution.",
        ISBN = "ATC044001",
        ListPrice = 70,
        Price = 65,
        Price50 = 60,
        Price100 = 55,
        CategoryId = 1,
        ImageUrl = ""
    },
    new()
    {
        Title = "One Hundred Years of Solitude",
        Author = "Gabriel García Márquez",
        Description = "A landmark novel in magical realism, following the Buendía family over several generations.",
        ISBN = "OHY045001",
        ListPrice = 75,
        Price = 70,
        Price50 = 65,
        Price100 = 60,
        CategoryId = 1,
        ImageUrl = ""
    }
            );

            try
            {
                await dbContext.SaveChangesAsync();
            }

            catch (Exception ex)
            {

            }
        }
    }



}
