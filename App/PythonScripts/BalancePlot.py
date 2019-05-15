import matplotlib.pyplot as plt
import matplotlib.dates as mdates
import matplotlib
import sys
import datetime
import gc

args = sys.argv[1:]
if( len(args) <= 0):
    sys.exit()
number_of_records = int(args[0])
args = args[1:]
dates = args[0::3]
earnings = args[1::3]
expenses =args[2::3]
earnings = [float(x.replace(',', '.')) for x in earnings]
expenses = [float(x.replace(',', '.')) for x in expenses]
dates = [datetime.datetime.strptime(d, '%d/%m/%Y').date() for d in dates]

matplotlib.rcParams['toolbar'] = 'None'

x = dates
y1, y2 = earnings, expenses
plt.gca().xaxis.set_major_formatter(mdates.DateFormatter('%m/%d/%Y'))
plt.gca().xaxis.set_major_locator(mdates.DayLocator())
plt.gcf().set_size_inches(8,6)
plt.plot(x, y1,lw=3, marker='o', markersize = 3, color = 'green')
plt.plot(x, y2,lw=3, marker='o', markersize = 1, color = 'red')
plt.title('Daily gold balance in RuneScape')
plt.xlabel('Date')
plt.ylabel('Balance')
plt.gcf().autofmt_xdate()

plt.show()

