import matplotlib.pyplot as plt
import sys
import datetime
import matplotlib.dates as mdates
import matplotlib
import gc

args = sys.argv[1:]
if (len(args) <= 0):
    sys.exit()
number_of_dates = int(args[0])
dates = args[1 : number_of_dates + 1]
values_index = number_of_dates + 1
number_of_values = int(args[values_index])
values = args[values_index + 1: values_index + number_of_values + 1]
values = [float(x.replace(',', '.')) for x in values]

dates = [datetime.datetime.strptime(d, "%d/%m/%Y").date() for d in dates]

x, y = dates,values

matplotlib.rcParams['toolbar'] = 'None'
plt.gca().xaxis.set_major_formatter(mdates.DateFormatter('%m/%d/%Y'))
plt.gca().xaxis.set_major_locator(mdates.DayLocator())
plt.gcf().set_size_inches(8,6)
plt.plot(x, y, lw = 2, color = '#ffd700')
plt.title('Daily Earnings')
plt.xlabel("Date")
plt.ylabel("Earnings")
plt.gcf().autofmt_xdate()

gc.collect()

plt.show()
