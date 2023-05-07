USE [StoreDB]
GO

--Select State, Sum(P.Sales) Sales from Orders O
--	Join Products P on O.OrderId = P.OrderId
--	WHERE YEAR(CONVERT(date, O.OrderDate, 120)) = 2018 and P.OrderId not in (Select OrderId from OrderReturns)
--	Group  By State
SELECT * FROM Orders WHERE TRY_CONVERT(date, OrderDate, 120) IS NULL
