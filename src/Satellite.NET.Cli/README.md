# Command line interface


#### Build and run
1. Build Satellite.NET.Cli
2. Navigate to the folder containing Satellite.net.cli.dll (`\Satellite.NET\src\Satellite.NET.Cli\bin\Debug\netcoreapp2.2` on Windows)  
3. Execute a command by running `dotnet Satellite.net.cli.dll <command> <options>`

#### Menu
```
Usage:
  Satellite.NET.Cli [options] [command]

Options:
  --version    Display version information

Commands:                                                                                                                
  create-order            Place an order for a message transmission                                                      
  cancel-order            Cancel an order                                                                                
  get-order               Retrieve an order                                                                              
  bump-order              Increase the bid for an order sitting in the transmission queue                                
  queued-orders           Retrieve a list of paid, but unsent orders in descending order of bid-per-byte                 
  pending-orders          Retrieve a list of 20 orders awaiting payment sorted in reverse chronological order            
  sent-orders             Retrieve a list of 20 paid orders sorted in reverse chronological order                        
  msg                     Retrieve the message sent in an order                                                          
  info                    Return information about the c-lightning node where satellite API payments are terminated      
  stream-transmissions    Subscribe to server-sent events for transmission messages.
```

#### Additional commands options

##### --pretty
Returns the result as a nicely formatted JSON.

##### --test
Calls the test API (the main API is used by default).

##### --url
Use if you want to call a custom API at the specified URL.

#### Examples
To show the global help menu: `dotnet Satellite.net.cli.dll --help`  
To show a command's help menu: `dotnet Satellite.net.cli.dll bump-order --help`  
To get a list of sent orders: `dotnet Satellite.net.cli.dll sent-orders`   
To get a list of sent orders, nicely formatted:  `dotnet Satellite.net.cli.dll sent-orders --pretty`  
To get a list of sent orders, nicely formatted, from the test API:  `dotnet Satellite.net.cli.dll sent-orders --pretty --test`
