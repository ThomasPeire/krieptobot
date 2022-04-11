WITH recommendatorLogs AS (
    SELECT    
		TimeStamp,
		CAST(Properties AS xml).query('(/properties/property[@key="CorrelationId"])').value('.', 'nvarchar(max)') as CorrelationId,
        CAST(Properties AS xml).query('(/properties/property[@key="Market"])').value('.', 'nvarchar(max)') as Market,
        CAST(Properties AS xml).query('(/properties/property[@key="Recommendator"])').value('.', 'nvarchar(max)') as Recommendator,
        CAST(Properties AS xml).query('(/properties/property[@key="Score"])').value('.', 'nvarchar(max)') as Score,
        CAST(Properties AS xml).query('(/properties/property[@key="MacdValue"])').value('.', 'nvarchar(max)') as MacdValue,
        CAST(Properties AS xml).query('(/properties/property[@key="PreviousValue"])').value('.', 'nvarchar(max)') as PreviousValue,
        CAST(Properties AS xml).query('(/properties/property[@key="CurrentValue"])').value('.', 'nvarchar(max)') as CurrentValue,
        CAST(Properties AS xml).query('(/properties/property[@key="RsiValue"])').value('.', 'nvarchar(max)') as RsiValue,
        CAST(Properties AS xml).query('(/properties/property[@key="Profit"])').value('.', 'nvarchar(max)') as Profit
    FROM    
        logs
),
 SellOrBuyLogs AS(
    SELECT    
		TimeStamp,
		CAST(Properties AS xml).query('(/properties/property[@key="CorrelationId"])').value('.', 'nvarchar(max)') as CorrelationId,
        CAST(Properties AS xml).query('(/properties/property[@key="Market"])').value('.', 'nvarchar(max)') as Market,
		CASE 
			WHEN CAST(Properties AS xml).query('(/properties/property[@key="SourceContext"])').value('.', 'nvarchar(max)')  = 'KrieptoBot.Application.SellManager' THEN 'Sell'
			WHEN CAST(Properties AS xml).query('(/properties/property[@key="SourceContext"])').value('.', 'nvarchar(max)')  = 'KrieptoBot.Application.BuyManager' THEN 'Buy'
			ELSE ''
		END as BuySell,
        CAST(Properties AS xml).query('(/properties/property[@key="Score"])').value('.', 'nvarchar(max)') as Score,
        CAST(Properties AS xml).query('(/properties/property[@key="Price"])').value('.', 'nvarchar(max)') as Price,
        CAST(Properties AS xml).query('(/properties/property[@key="Amount"])').value('.', 'nvarchar(max)') as Amount,
        CAST(Properties AS xml).query('(/properties/property[@key="Euro"])').value('.', 'nvarchar(max)') as Total,
        CAST(Properties AS xml).query('(/properties/property[@key="Budget"])').value('.', 'nvarchar(max)') as Budget
    FROM    
        logs 
	WHERE CAST(Properties AS xml).query('(/properties/property[@key="SourceContext"])').value('.', 'nvarchar(max)')  = 'KrieptoBot.Application.BuyManager'
	OR CAST(Properties AS xml).query('(/properties/property[@key="SourceContext"])').value('.', 'nvarchar(max)')  = 'KrieptoBot.Application.SellManager'
)

Select * from SellOrBuyLogs