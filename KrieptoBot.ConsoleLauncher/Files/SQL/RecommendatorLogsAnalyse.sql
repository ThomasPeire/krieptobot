SELECT
    TimeStamp, CAST (Properties AS xml).query('(/properties/property[@key="CorrelationId"])').value('.', 'nvarchar(max)') as CorrelationId, CAST (Properties AS xml).query('(/properties/property[@key="Market"])').value('.', 'nvarchar(max)') as Market, CASE WHEN CAST (Properties AS xml).query('(/properties/property[@key="Recommendator"])').value('.', 'nvarchar(max)') = ''
    THEN 'Final score'
    ELSE CAST (Properties AS xml).query('(/properties/property[@key="Recommendator"])').value('.', 'nvarchar(max)')
END
as Recommendator,
        CAST(Properties AS xml).query('(/properties/property[@key="Score"])').value('.', 'nvarchar(max)') as Score,
        CAST(Properties AS xml).query('(/properties/property[@key="MacdValue"])').value('.', 'nvarchar(max)') as MacdValue,
        CAST(Properties AS xml).query('(/properties/property[@key="PreviousValue"])').value('.', 'nvarchar(max)') as PreviousValue,
        CAST(Properties AS xml).query('(/properties/property[@key="CurrentValue"])').value('.', 'nvarchar(max)') as CurrentValue,
        CAST(Properties AS xml).query('(/properties/property[@key="RsiValue"])').value('.', 'nvarchar(max)') as RsiValue,
        CAST(Properties AS xml).query('(/properties/property[@key="Profit"])').value('.', 'nvarchar(max)') as Profit
    FROM    
        logs
		WHERE (CAST(Properties AS xml).query('(/properties/property[@key="Recommendator"])').value('.', 'nvarchar(max)') != '' OR
		CAST(Properties AS xml).query('(/properties/property[@key="Score"])').value('.', 'nvarchar(max)') != '')