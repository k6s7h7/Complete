BEGIN_FUNCTION_MAP
	.Func,CME/EUREX��������ȸ(API��)(t8437),t8437,block,headtype=A;
	BEGIN_DATA_MAP
	t8437InBlock,�⺻�Է�,input;
	begin
		����(NF/NC/NM/NO),gubun,gubun,char,2;
	end
	t8437OutBlock,�߰����帶����,output,occurs;
	begin
		�����,hname,hname,char,20;
		�����ڵ�,shcode,shcode,char,8;
		ǥ���ڵ�,expcode,expcode,char,12;
		�ŷ��¼�,tradeunit,tradeunit,float,21.8;
		ATM����(1:ATM2:ITM3:OTM),atmgb,atmgb,char,1;
	end
	END_DATA_MAP
END_FUNCTION_MAP

