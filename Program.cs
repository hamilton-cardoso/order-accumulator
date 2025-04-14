using OrderAccumulator.Fix;
using QuickFix;
using QuickFix.Logger;
using QuickFix.Store;


var settings = new SessionSettings("Fix/acceptor.cfg");
var app = new FixServerApp();
var storeFactory = new FileStoreFactory(settings);
var logFactory = new FileLogFactory(settings);
var acceptor = new QuickFix.ThreadedSocketAcceptor(app, storeFactory, settings, logFactory);


acceptor.Start();

Console.WriteLine("OrderAccumulator FIX server iniciado.");
Console.WriteLine("Aguardando ordens... Pressione ENTER para sair.");
Console.ReadLine();

acceptor.Stop();
